using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UM_with_Role_Claim_Audit_log.Models.ViewModels;
using UM_with_Role_Claim_Audit_log.Services;

namespace UM_with_Role_Claim_Audit_log.Controllers
{
    /// <summary>
    /// Controller responsible for managing user claims in the application.
    /// Handles viewing, assigning, and updating claims for individual users.
    /// Requires authentication for all actions.
    /// </summary>
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuditLogService _auditLogService;

        /// <summary>
        /// Initializes a new instance of the ClaimsController with required dependencies.
        /// </summary>
        /// <param name="userManager">Service for managing user accounts</param>
        /// <param name="roleManager">Service for managing roles</param>
        /// <param name="auditLogService">Service for logging audit trail activities</param>
        public ClaimsController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AuditLogService auditLogService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Determines if the current user has permission to manage claims for a target user.
        /// Admin users can manage all users. Non-admin users cannot manage Admin accounts.
        /// </summary>
        /// <param name="targetUserId">The ID of the user whose claims are being managed</param>
        /// <returns>True if the current user can manage the target user's claims, false otherwise</returns>
        private async Task<bool> CanManageUserClaims(string targetUserId)
        {
            // Admins have unrestricted access to manage any user's claims
            if (User.IsInRole("Admin"))
                return true;

            // Retrieve the target user from the database
            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
                return false;

            // Get all roles assigned to the target user
            var targetUserRoles = await _userManager.GetRolesAsync(targetUser);

            // Non-admin users cannot manage claims for Admin users (security restriction)
            if (targetUserRoles.Contains("Admin"))
                return false;

            return true;
        }

        /// <summary>
        /// Displays the claims management page for a specific user.
        /// Shows all available claims with their current status (assigned, inherited from role, etc.).
        /// Special handling for Admin users: all claims are auto-selected and displayed as enabled.
        /// </summary>
        /// <param name="id">The user ID whose claims will be managed</param>
        /// <returns>View with claims management interface or error redirect</returns>
        [Authorize(Policy = "CanViewClaimsPolicy")]
        public async Task<IActionResult> Manage(string id)
        {
            // Validate that a user ID was provided
            if (string.IsNullOrEmpty(id))
                return NotFound();

            // Check if current user has permission to manage this user's claims
            if (!await CanManageUserClaims(id))
            {
                TempData["Error"] = "You do not have permission to view claims for this user.";
                return RedirectToAction("Index", "Users");
            }

            // Retrieve the target user
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Get all claims directly assigned to this user
            var userClaims = await _userManager.GetClaimsAsync(user);

            // Get all roles assigned to this user
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleClaimsList = new List<Claim>();

            // Check if user is Admin (Admin users have special claim handling)
            bool isAdminUser = userRoles.Contains("Admin");

            // Collect all claims inherited from the user's roles
            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    roleClaimsList.AddRange(roleClaims);
                }
            }

            // Build the view model with all available claims and their current status
            var viewModel = new ManageClaimsViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email ?? "",
                Claims = ClaimDefinitionsService.GetAllClaims().Select(kvp =>
                {
                    // Check if this claim is inherited from any of the user's roles
                    var hasRoleClaim = roleClaimsList.Any(c => c.Type == kvp.Key && c.Value == "true");

                    // Check if this claim is directly assigned to the user
                    var hasUserClaim = userClaims.Any(c => c.Type == kvp.Key && c.Value == "true");

                    return new UserClaimViewModel
                    {
                        ClaimType = kvp.Key,
                        ClaimValue = "true",
                        Description = kvp.Value,
                        // Admin users automatically have all claims selected
                        IsSelected = isAdminUser || hasUserClaim,
                        // Mark claims that come from roles (for UI display)
                        IsInheritedFromRole = hasRoleClaim,
                        // Claims inherited from roles cannot be modified unless also directly assigned
                        IsReadOnly = hasRoleClaim && !hasUserClaim
                    };
                }).ToList()
            };

            // Determine if current user can modify claims (not just view)
            var canAssignClaims = User.IsInRole("Admin") ||
                User.HasClaim(c => (c.Type == "CanAssignClaims" || c.Type == "CanManageClaims") && c.Value == "true");

            // Pass permission flags to the view for UI rendering
            ViewBag.CanAssignClaims = canAssignClaims;
            ViewBag.IsReadOnly = !canAssignClaims;
            ViewBag.IsAdminUser = isAdminUser;

            return View(viewModel);
        }

        /// <summary>
        /// Processes the claims management form submission.
        /// Updates the user's claims based on the selections made in the form.
        /// Logs all changes to the audit log for compliance and security tracking.
        /// </summary>
        /// <param name="model">View model containing the user ID and selected claims</param>
        /// <returns>Redirect to user details on success, or back to form with error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanAssignClaimsPolicy")]
        public async Task<IActionResult> Manage(ManageClaimsViewModel model)
        {
            // Validate the submitted form data
            if (!ModelState.IsValid)
                return View(model);

            // Re-check permissions (defense in depth - verify on POST as well as GET)
            if (!await CanManageUserClaims(model.UserId))
            {
                TempData["Error"] = "You do not have permission to manage claims for this user.";
                return RedirectToAction("Index", "Users");
            }

            // Retrieve the target user
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound();

            // Get all current claims for this user
            var userClaims = await _userManager.GetClaimsAsync(user);

            // Filter to only system-defined claims (ignore any custom claims)
            var systemClaims = userClaims.Where(c => ClaimDefinitionsService.IsValidClaim(c.Type)).ToList();

            // Remove all existing system claims before adding new ones (clean slate approach)
            if (systemClaims.Any())
            {
                var removeResult = await _userManager.RemoveClaimsAsync(user, systemClaims);
                if (!removeResult.Succeeded)
                {
                    TempData["Error"] = "Error removing existing claims.";
                    return RedirectToAction("Manage", new { id = model.UserId });
                }
            }

            // Prepare the new set of claims based on form selections
            var selectedClaims = model.Claims
                .Where(c => c.IsSelected)
                .Select(c => new Claim(c.ClaimType, c.ClaimValue))
                .ToList();

            // Add the newly selected claims to the user
            if (selectedClaims.Any())
            {
                var addResult = await _userManager.AddClaimsAsync(user, selectedClaims);
                if (!addResult.Succeeded)
                {
                    TempData["Error"] = "Error adding new claims.";
                    return RedirectToAction("Manage", new { id = model.UserId });
                }
            }

            // Log this activity to the audit trail for security and compliance
            var currentUserId = _userManager.GetUserId(User);
            var claimsList = string.Join(", ", selectedClaims.Select(c => c.Type));
            await _auditLogService.LogAsync(
                currentUserId ?? "System",
                "Claims Updated",
                $"Updated claims for user: {user.Email}. Claims: {claimsList}"
            );

            // Show success message and redirect to user details page
            TempData["Success"] = "User claims updated successfully.";
            return RedirectToAction("Details", "Users", new { id = model.UserId });
        }

        /// <summary>
        /// Displays a page showing all available claims in the system with their descriptions.
        /// Useful for administrators to understand what permissions are available.
        /// </summary>
        /// <returns>View listing all defined claims in the system</returns>
        [Authorize(Policy = "CanViewClaimsPolicy")]
        public IActionResult Available()
        {
            return View(ClaimDefinitionsService.AvailableClaims);
        }
    }
}