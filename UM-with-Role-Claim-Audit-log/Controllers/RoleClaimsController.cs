using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UM_with_Role_Claim_Audit_log.Models.ViewModels;
using UM_with_Role_Claim_Audit_log.Services;

namespace UM_with_Role_Claim_Audit_log.Controllers
{
    /// <summary>
    /// Controller responsible for managing claims assigned to roles.
    /// Handles viewing and updating role-based claims which are inherited by all users in that role.
    /// Requires CanManageRoleClaimsPolicy authorization for all actions.
    /// </summary>
    [Authorize(Policy = "CanManageRoleClaimsPolicy")]
    public class RoleClaimsController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuditLogService _auditLogService;

        /// <summary>
        /// Initializes a new instance of the RoleClaimsController with required dependencies.
        /// </summary>
        /// <param name="roleManager">Service for managing roles and their claims</param>
        /// <param name="userManager">Service for managing users (used for audit logging)</param>
        /// <param name="auditLogService">Service for logging audit trail activities</param>
        public RoleClaimsController(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            AuditLogService auditLogService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Displays the role claims management page for a specific role.
        /// Shows all available system claims with their current assignment status.
        /// Special handling for Admin role: all claims are auto-selected and displayed as enabled.
        /// Role-based claims are inherited by all users who have that role assigned.
        /// </summary>
        /// <param name="id">The role ID whose claims will be managed</param>
        /// <returns>View with role claims management interface or NotFound if role doesn't exist</returns>
        public async Task<IActionResult> Manage(string id)
        {
            // Validate that a role ID was provided
            if (string.IsNullOrEmpty(id))
                return NotFound();

            // Retrieve the target role from the database
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            // Get all claims currently assigned to this role
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            // Check if this is the Admin role (Admin role has special claim handling)
            bool isAdminRole = role.Name?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;

            // Build the view model using the centralized ClaimDefinitionsService
            // This ensures consistency across the application for claim definitions
            var viewModel = new ManageRoleClaimsViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name ?? "",
                // Map all system-defined claims to view model items
                Claims = ClaimDefinitionsService.GetAllClaims().Select(kvp =>
                {
                    // Check if this role currently has this claim assigned
                    var hasClaim = roleClaims.Any(c => c.Type == kvp.Key && c.Value == "true");

                    // Create view model for this claim
                    return new UserClaimViewModel // Note: Reusing UserClaimViewModel for consistency
                    {
                        ClaimType = kvp.Key,
                        ClaimValue = "true",
                        Description = kvp.Value, // Description automatically loaded from ClaimDefinitionsService
                        // Admin role automatically has all claims selected (administrative override)
                        IsSelected = isAdminRole || hasClaim
                    };
                }).ToList()
            };

            // Pass Admin role flag to view for special UI rendering (e.g., disable checkboxes)
            ViewBag.IsAdminRole = isAdminRole;

            return View(viewModel);
        }

        /// <summary>
        /// Processes the role claims management form submission.
        /// Updates the role's claims based on the selections made in the form.
        /// All users with this role will inherit these claims automatically.
        /// Logs all changes to the audit log for compliance and security tracking.
        /// </summary>
        /// <param name="model">View model containing the role ID and selected claims</param>
        /// <returns>Redirect to role details on success, or back to form with validation errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(ManageRoleClaimsViewModel model)
        {
            // Validate the submitted form data
            if (!ModelState.IsValid)
                return View(model);

            // Retrieve the target role
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
                return NotFound();

            // Get all current claims for this role
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            // Filter to only system-defined claims using the ClaimDefinitionsService
            // This prevents accidental removal of custom claims that may exist
            var systemClaims = roleClaims.Where(c => ClaimDefinitionsService.IsValidClaim(c.Type)).ToList();

            // Remove all existing system claims (clean slate approach)
            // This ensures the role only has the claims selected in the form
            foreach (var claim in systemClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            // Prepare the new set of claims based on form selections
            var selectedClaims = model.Claims
                .Where(c => c.IsSelected)
                .Select(c => new Claim(c.ClaimType, c.ClaimValue))
                .ToList();

            // Add all selected claims to the role
            // These claims will be inherited by all users who have this role
            foreach (var claim in selectedClaims)
            {
                await _roleManager.AddClaimAsync(role, claim);
            }

            // Log this activity to the audit trail for security and compliance
            var currentUserId = _userManager.GetUserId(User);
            var claimsList = string.Join(", ", selectedClaims.Select(c => c.Type));
            await _auditLogService.LogAsync(
                currentUserId ?? "System",
                "Role Claims Updated",
                $"Updated claims for role: {role.Name}. Claims: {claimsList}"
            );

            // Show success message and redirect to role details page
            TempData["Success"] = "Role claims updated successfully.";
            return RedirectToAction("Details", "Roles", new { id = model.RoleId });
        }
    }
}