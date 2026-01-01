namespace UM_with_Role_Claim_Audit_log.Models.ViewModels
{
    /// <summary>
    /// View model for assigning or removing roles from a user.
    /// Used in the Edit User page to display available roles with checkboxes.
    /// Supports multi-role assignment - users can have multiple roles simultaneously.
    /// </summary>
    public class AssignRoleViewModel
    {
        /// <summary>
        /// The unique identifier of the user being assigned roles.
        /// Maps to AspNetUsers.Id in the database.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the user for display purposes.
        /// Shown in the UI to confirm which user is being modified.
        /// </summary>
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>
        /// Collection of all available roles in the system with their selection status.
        /// Each role is represented by a RoleSelectionViewModel with a checkbox in the UI.
        /// Selected roles will be assigned to the user; unselected roles will be removed.
        /// </summary>
        public List<RoleSelectionViewModel> Roles { get; set; } = new();
    }

    /// <summary>
    /// Represents a single role with its selection state in the role assignment UI.
    /// Used within AssignRoleViewModel to display checkboxes for each available role.
    /// </summary>
    public class RoleSelectionViewModel
    {
        /// <summary>
        /// The name of the role (e.g., "Admin", "Manager", "User").
        /// Maps to AspNetRoles.Name in the database.
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether this role is currently assigned to the user.
        /// True = checkbox is checked (user has this role)
        /// False = checkbox is unchecked (user does not have this role)
        /// </summary>
        public bool IsSelected { get; set; }
    }
}