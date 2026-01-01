using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UM_with_Role_Claim_Audit_log.Models.ViewModels;
using UM_with_Role_Claim_Audit_log.Services;

namespace UM_with_Role_Claim_Audit_log.Controllers
{
    /// <summary>
    /// Controller responsible for comprehensive user management operations.
    /// Handles user CRUD operations, role assignments, account status management, and password resets.
    /// Implements hierarchical authorization where Admin users have full access and
    /// non-Admin users cannot manage Admin accounts.
    /// All actions require authentication.
    /// </summary>
    [Authorize] // Requires login for all controllers
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuditLogService _auditLogService;

        /// <summary>
        /// Constructor to inject required services.
        /// </summary>
        /// <param name="userManager">ASP.NET Core Identity UserManager for user operations</param>
        /// <param name="roleManager">ASP.NET Core Identity RoleManager for role operations</param>
        /// <param name="auditLogService">Custom service for logging user actions</param>
        public UsersController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AuditLogService auditLogService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Helper method to validate whether the current user has permission to manage a target user.
        /// Implements hierarchical security: Admin can manage anyone, but non-Admin cannot manage Admin users.
        /// </summary>
        /// <param name="targetUserId">The ID of the user to be managed</param>
        /// <returns>True if the current user can manage the target user, false otherwise</returns>
        private async Task<bool> CanManageUser(string targetUserId)
        {
            // Admin can manage anyone
            if (User.IsInRole("Admin"))
                return true;

            // Non-Admin cannot manage Admin users
            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
                return false;

            var targetUserRoles = await _userManager.GetRolesAsync(targetUser);

            // If target user is Admin, only Admin can manage
            if (targetUserRoles.Contains("Admin"))
                return false;

            return true;
        }

        /// <summary>
        /// GET: Users
        /// Displays a list of all users in the system with their basic information and assigned roles.
        /// Non-Admin users cannot see Admin users in the list (security feature).
        /// Requires "CanEditUsersPolicy" authorization.
        /// </summary>
        /// <returns>View with list of users (filtered based on current user's role)</returns>
        [Authorize(Policy = "CanViewUsersPolicy")]
        public async Task<IActionResult> Index()
        {
            // Retrieve all users from the database
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Hide Admin users from non-Admin users (security feature)
                if (!User.IsInRole("Admin") && roles.Contains("Admin"))
                    continue;

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        /// <summary>
        /// GET: Users/Details/{id}
        /// Displays detailed information about a specific user including email, roles, and claims.
        /// Non-Admin users cannot view details of Admin users.
        /// Requires "CanEditUsersPolicy" authorization.
        /// </summary>
        /// <param name="id">The ID of the user to view</param>
        /// <returns>View with detailed user information or error redirect</returns>
        [Authorize(Policy = "CanEditUsersPolicy")]
        public async Task<IActionResult> Details(string id)
        {
            // Validate the user ID parameter
            if (id == null)
                return NotFound();

            // Check if current user has permission to view this user
            if (!await CanManageUser(id))
            {
                TempData["Error"] = "You do not have permission to view this user.";
                return RedirectToAction(nameof(Index));
            }

            // Retrieve the user from the database
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Get user's roles and claims
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                Roles = roles.ToList(),
                Claims = claims.Select(c => $"{c.Type}: {c.Value}").ToList()
            };

            return View(viewModel);
        }

        /// <summary>
        /// GET: Users/Create
        /// Displays the user creation form.
        /// Requires "CanEditUsersPolicy" authorization.
        /// </summary>
        /// <returns>View with empty user creation form</returns>
        [Authorize(Policy = "CanEditUsersPolicy")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: Users/Create
        /// Processes the user creation form submission.
        /// Creates a new user account with the specified email and password.
        /// Requires "CanEditUsersPolicy" authorization.
        /// Logs the creation action to the audit log.
        /// </summary>
        /// <param name="model">The view model containing new user information</param>
        /// <returns>Redirect to Index on success or back to Create view with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanEditUsersPolicy")]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            // Validate the model state
            if (ModelState.IsValid)
            {
                // Create new Identity user
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = model.EmailConfirmed
                };

                // Create user with password
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Log the user creation action to the audit log
                    var currentUserId = _userManager.GetUserId(User);
                    await _auditLogService.LogAsync(
                        currentUserId ?? "System",
                        "User Created",
                        $"Created user: {user.Email}"
                    );

                    TempData["Success"] = "User created successfully.";
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
        /// GET: Users/Edit/{id}
        /// Displays the user editing form with current user information and role assignments.
        /// Non-Admin users cannot edit Admin users and cannot see Admin role in the dropdown.
        /// Requires "CanEditUsersPolicy" authorization.
        /// </summary>
        /// <param name="id">The ID of the user to edit</param>
        /// <returns>View with user edit form or error redirect</returns>
        [Authorize(Policy = "CanEditUsersPolicy")]
        public async Task<IActionResult> Edit(string id)
        {
            // Validate the user ID parameter
            if (id == null)
                return NotFound();

            // Check if current user has permission to edit this user
            if (!await CanManageUser(id))
            {
                TempData["Error"] = "You do not have permission to edit this user.";
                return RedirectToAction(nameof(Index));
            }

            // Retrieve the user from the database
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Get user's current roles and all available roles
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.Select(r => r.Name ?? "").ToListAsync();

            // Non-Admin users cannot see Admin role in dropdown (security feature)
            if (!User.IsInRole("Admin"))
            {
                allRoles = allRoles.Where(r => r != "Admin").ToList();
            }

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                EmailConfirmed = user.EmailConfirmed,
                SelectedRoles = userRoles.ToList(),
                AllRoles = allRoles
            };

            return View(viewModel);
        }

        /// <summary>
        /// POST: Users/Edit/{id}
        /// Processes the user editing form submission.
        /// Updates user information and role assignments.
        /// Non-Admin users cannot assign the Admin role (security feature).
        /// Requires "CanEditUsersPolicy" authorization.
        /// Logs the update action to the audit log.
        /// </summary>
        /// <param name="id">The ID of the user to edit</param>
        /// <param name="model">The view model containing updated user information</param>
        /// <returns>Redirect to Index on success or back to Edit view with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanEditUsersPolicy")]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            // Validate ID match
            if (id != model.Id)
                return NotFound();

            // Check if current user has permission to edit this user
            if (!await CanManageUser(id))
            {
                TempData["Error"] = "You do not have permission to edit this user.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return NotFound();

                // Prevent non-Admin from assigning Admin role (security feature)
                if (!User.IsInRole("Admin") && model.SelectedRoles.Contains("Admin"))
                {
                    TempData["Error"] = "You do not have permission to assign Admin role.";
                    model.AllRoles = await _roleManager.Roles
                        .Where(r => r.Name != "Admin")
                        .Select(r => r.Name ?? "")
                        .ToListAsync();
                    return View(model);
                }

                // Update user information
                user.Email = model.Email;
                user.UserName = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Update user roles - calculate roles to add and remove
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var rolesToRemove = currentRoles.Except(model.SelectedRoles).ToList();
                    var rolesToAdd = model.SelectedRoles.Except(currentRoles).ToList();

                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    await _userManager.AddToRolesAsync(user, rolesToAdd);

                    // Log the user update action to the audit log
                    var currentUserId = _userManager.GetUserId(User);
                    await _auditLogService.LogAsync(
                        currentUserId ?? "System",
                        "User Updated",
                        $"Updated user: {user.Email}"
                    );

                    TempData["Success"] = "User updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                // Add any errors to the model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Reload roles list for the view (filtered based on current user's role)
            model.AllRoles = await _roleManager.Roles
                .Select(r => r.Name ?? "")
                .Where(r => User.IsInRole("Admin") || r != "Admin")
                .ToListAsync();
            return View(model);
        }

        /// <summary>
        /// GET: Users/Delete/{id}
        /// Displays the user deletion confirmation page.
        /// Non-Admin users cannot delete Admin users.
        /// Requires "CanDeleteUsersPolicy" authorization.
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>View with user deletion confirmation or error redirect</returns>
        [Authorize(Policy = "CanDeleteUsersPolicy")]
        public async Task<IActionResult> Delete(string id)
        {
            // Validate the user ID parameter
            if (id == null)
                return NotFound();

            // Check if current user has permission to delete this user
            if (!await CanManageUser(id))
            {
                TempData["Error"] = "You do not have permission to delete this user.";
                return RedirectToAction(nameof(Index));
            }

            // Retrieve the user from the database
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList()
            };

            return View(viewModel);
        }

        /// <summary>
        /// POST: Users/Delete/{id}
        /// Processes the user deletion confirmation.
        /// Permanently removes the user account from the system.
        /// Non-Admin users cannot delete Admin users.
        /// Requires "CanDeleteUsersPolicy" authorization.
        /// Logs the deletion action to the audit log.
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>Redirect to Index with success/error message</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanDeleteUsersPolicy")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Check if current user has permission to delete this user
            if (!await CanManageUser(id))
            {
                TempData["Error"] = "You do not have permission to delete this user.";
                return RedirectToAction(nameof(Index));
            }

            // Retrieve the user from the database
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Delete the user
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                // Log the user deletion action to the audit log
                var currentUserId = _userManager.GetUserId(User);
                await _auditLogService.LogAsync(
                    currentUserId ?? "System",
                    "User Deleted",
                    $"Deleted user: {user.Email}"
                );

                TempData["Success"] = "User deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Error deleting user.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// POST: Users/ToggleStatus/{id}
        /// Enables or disables a user account by setting/removing lockout.
        /// Disabled users cannot log in until re-enabled.
        /// Non-Admin users cannot disable/enable Admin users.
        /// Requires "CanDisableUsersPolicy" authorization.
        /// Logs the status change action to the audit log.
        /// </summary>
        /// <param name="id">The ID of the user whose status to toggle</param>
        /// <returns>Redirect to Index with success/error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanDisableUsersPolicy")]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            // Check if current user has permission to disable/enable this user
            if (!await CanManageUser(id))
            {
                TempData["Error"] = "You do not have permission to disable/enable this user.";
                return RedirectToAction(nameof(Index));
            }

            // Retrieve the user from the database
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Check current lockout status and toggle it
            if (user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.Now)
            {
                // Disable user by setting lockout to maximum date
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                var currentUserId = _userManager.GetUserId(User);
                await _auditLogService.LogAsync(
                    currentUserId ?? "System",
                    "User Disabled",
                    $"Disabled user: {user.Email}"
                );
                TempData["Success"] = "User disabled successfully.";
            }
            else
            {
                // Enable user by removing lockout
                await _userManager.SetLockoutEndDateAsync(user, null);
                var currentUserId = _userManager.GetUserId(User);
                await _auditLogService.LogAsync(
                    currentUserId ?? "System",
                    "User Enabled",
                    $"Enabled user: {user.Email}"
                );
                TempData["Success"] = "User enabled successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// GET: Users/ResetPassword/{id}
        /// Displays the password reset form for a specific user.
        /// Non-Admin users cannot reset passwords for Admin users.
        /// Requires "CanResetPasswordsPolicy" authorization.
        /// </summary>
        /// <param name="id">The ID of the user whose password to reset</param>
        /// <returns>View with password reset form or error redirect</returns>
        [Authorize(Policy = "CanResetPasswordsPolicy")]
        public async Task<IActionResult> ResetPassword(string id)
        {
            // Validate the user ID parameter
            if (id == null)
                return NotFound();

            // Check if current user has permission to reset password for this user
            if (!await CanManageUser(id))
            {
                TempData["Error"] = "You do not have permission to reset password for this user.";
                return RedirectToAction(nameof(Index));
            }

            // Retrieve the user from the database
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var viewModel = new ResetPasswordViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? ""
            };

            return View(viewModel);
        }

        /// <summary>
        /// POST: Users/ResetPassword
        /// Processes the password reset form submission.
        /// Generates a password reset token and sets the new password.
        /// Non-Admin users cannot reset passwords for Admin users.
        /// Requires "CanResetPasswordsPolicy" authorization.
        /// Logs the password reset action to the audit log.
        /// </summary>
        /// <param name="model">The view model containing the user ID and new password</param>
        /// <returns>Redirect to Index on success or back to ResetPassword view with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanResetPasswordsPolicy")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            // Check if current user has permission to reset password for this user
            if (!await CanManageUser(model.UserId))
            {
                TempData["Error"] = "You do not have permission to reset password for this user.";
                return RedirectToAction(nameof(Index));
            }

            // Validate the model state
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                    return NotFound();

                // Generate password reset token and reset the password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (result.Succeeded)
                {
                    // Log the password reset action to the audit log
                    var currentUserId = _userManager.GetUserId(User);
                    await _auditLogService.LogAsync(
                        currentUserId ?? "System",
                        "Password Reset",
                        $"Reset password for user: {user.Email}"
                    );

                    TempData["Success"] = "Password reset successfully.";
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
    }
}