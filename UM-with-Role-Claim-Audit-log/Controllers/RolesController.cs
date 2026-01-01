using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UM_with_Role_Claim_Audit_log.Models.ViewModels;
using UM_with_Role_Claim_Audit_log.Services;

namespace UM_with_Role_Claim_Audit_log.Controllers
{
    /// <summary>
    /// Controller responsible for managing roles in the system.
    /// Handles creating, viewing, deleting roles and managing role assignments.
    /// Implements role-based and claims-based authorization for different operations.
    /// All actions require authentication (user must be logged in).
    /// </summary>
    [Authorize] // Requires authentication for all actions in this controller
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuditLogService _auditLogService;

        /// <summary>
        /// Constructor to inject required services.
        /// </summary>
        /// <param name="roleManager">ASP.NET Core Identity RoleManager for role operations</param>
        /// <param name="userManager">ASP.NET Core Identity UserManager for user operations</param>
        /// <param name="auditLogService">Custom service for logging user actions</param>
        public RolesController(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            AuditLogService auditLogService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// GET: Roles
        /// Displays a list of all roles in the system with their user counts.
        /// Requires "CanViewRolesPolicy" authorization (read-only access).
        /// Also checks for create/delete permissions to show/hide action buttons in the view.
        /// </summary>
        /// <returns>View with list of roles and permission flags</returns>
        [Authorize(Policy = "CanViewRolesPolicy")]
        public async Task<IActionResult> Index()
        {
            // Retrieve all roles from the database
            var roles = await _roleManager.Roles.ToListAsync();
            var roleViewModels = new List<RoleViewModel>();

            // Build view models with user count for each role
            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name ?? "");
                roleViewModels.Add(new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name ?? "",
                    UserCount = usersInRole.Count
                });
            }

            // Pass permission information to the view to show/hide action buttons
            ViewBag.CanCreate = User.IsInRole("Admin") ||
                User.HasClaim(c => (c.Type == "CanCreateRoles" || c.Type == "CanManageRoles") && c.Value == "true");
            ViewBag.CanDelete = User.IsInRole("Admin") ||
                User.HasClaim(c => (c.Type == "CanDeleteRoles" || c.Type == "CanManageRoles") && c.Value == "true");

            return View(roleViewModels);
        }

        /// <summary>
        /// GET: Roles/Create
        /// Displays the role creation form.
        /// Requires "CanCreateRolesPolicy" authorization.
        /// </summary>
        /// <returns>View with empty role creation form</returns>
        [Authorize(Policy = "CanCreateRolesPolicy")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: Roles/Create
        /// Processes the role creation form submission.
        /// Validates that the role name is unique before creating.
        /// Requires "CanCreateRolesPolicy" authorization.
        /// Logs the creation action to the audit log.
        /// </summary>
        /// <param name="model">The role view model containing the new role name</param>
        /// <returns>Redirect to Index on success or back to Create view with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanCreateRolesPolicy")]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            // Validate the model state
            if (ModelState.IsValid)
            {
                // Check if role name already exists
                var roleExists = await _roleManager.RoleExistsAsync(model.Name);
                if (roleExists)
                {
                    ModelState.AddModelError(string.Empty, "Role already exists.");
                    return View(model);
                }

                // Create the new role
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));

                if (result.Succeeded)
                {
                    // Log the role creation action to the audit log
                    var currentUserId = _userManager.GetUserId(User);
                    await _auditLogService.LogAsync(
                        currentUserId ?? "System",
                        "Role Created",
                        $"Created role: {model.Name}"
                    );

                    TempData["Success"] = "Role created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                // Add any errors to the model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        /// <summary>
        /// GET: Roles/Delete/{id}
        /// Displays the role deletion confirmation page.
        /// Shows role details and number of users assigned to the role.
        /// Requires "CanDeleteRolesPolicy" authorization.
        /// </summary>
        /// <param name="id">The ID of the role to delete</param>
        /// <returns>View with role deletion confirmation or NotFound</returns>
        [Authorize(Policy = "CanDeleteRolesPolicy")]
        public async Task<IActionResult> Delete(string id)
        {
            // Validate the role ID parameter
            if (id == null)
                return NotFound();

            // Retrieve the role from the database
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            // Get the count of users assigned to this role
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name ?? "");

            var viewModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name ?? "",
                UserCount = usersInRole.Count
            };

            return View(viewModel);
        }

        /// <summary>
        /// POST: Roles/Delete/{id}
        /// Processes the role deletion confirmation.
        /// Permanently removes the role from the system.
        /// Requires "CanDeleteRolesPolicy" authorization.
        /// Logs the deletion action to the audit log.
        /// </summary>
        /// <param name="id">The ID of the role to delete</param>
        /// <returns>Redirect to Index with success/error message</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanDeleteRolesPolicy")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Retrieve the role from the database
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            // Delete the role
            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                // Log the role deletion action to the audit log
                var currentUserId = _userManager.GetUserId(User);
                await _auditLogService.LogAsync(
                    currentUserId ?? "System",
                    "Role Deleted",
                    $"Deleted role: {role.Name}"
                );

                TempData["Success"] = "Role deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Error deleting role.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// GET: Roles/Details/{id}
        /// Displays detailed information about a specific role.
        /// Shows the role name, assigned users, and associated claims.
        /// Requires "CanViewRolesPolicy" authorization (read-only access).
        /// Also checks for role claims management permission to show/hide edit options.
        /// </summary>
        /// <param name="id">The ID of the role to view</param>
        /// <returns>View with detailed role information or NotFound</returns>
        [Authorize(Policy = "CanViewRolesPolicy")]
        public async Task<IActionResult> Details(string id)
        {
            // Validate the role ID parameter
            if (id == null)
                return NotFound();

            // Retrieve the role from the database
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            // Get all users assigned to this role
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name ?? "");

            // Get all claims associated with this role
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            // Build the detailed view model
            var viewModel = new RoleDetailsViewModel
            {
                Id = role.Id,
                Name = role.Name ?? "",
                UserCount = usersInRole.Count,
                Users = usersInRole.Select(u => u.Email ?? "").ToList(),
                Claims = roleClaims.Select(c => $"{c.Type}: {c.Value}").ToList()
            };

            // Pass permission flag to the view to determine if role claims can be edited
            ViewBag.CanManageRoleClaims = User.IsInRole("Admin") ||
                User.HasClaim(c => c.Type == "CanManageRoleClaims" && c.Value == "true");

            return View(viewModel);
        }
    }
}