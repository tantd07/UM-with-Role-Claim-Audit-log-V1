namespace UM_with_Role_Claim_Audit_log.Models.ViewModels
{
    /// <summary>
    /// View model for displaying comprehensive role information on the role details page.
    /// Used in the Roles/Details page to show role metadata, assigned users, and role-based claims.
    /// Provides a centralized view of everything related to a specific role.
    /// 
    /// Key Features:
    /// - Displays role name and unique identifier
    /// - Shows count of users assigned to this role
    /// - Lists all users who have this role (for auditing and management)
    /// - Lists all claims (permissions) assigned to this role
    /// 
    /// Use Cases:
    /// - Role auditing: See which users belong to a role
    /// - Permission review: See what permissions a role grants
    /// - Impact analysis: Before deleting/modifying a role, see how many users are affected
    /// - Documentation: Generate reports of role assignments and permissions
    /// 
    /// Navigation Context:
    /// Users can navigate from this page to:
    /// - Edit role name (/Roles/Edit/{id})
    /// - Manage role claims (/RoleClaims/Manage/{id})
    /// - Edit individual users (/Users/Edit/{userId})
    /// - Delete role (/Roles/Delete/{id}) with user count warning
    /// </summary>
    public class RoleDetailsViewModel
    {
        /// <summary>
        /// The unique identifier of the role.
        /// Maps to AspNetRoles.Id in the database.
        /// Used for generating action links to edit, delete, or manage claims for this role.
        /// Typically a GUID string (e.g., "a1b2c3d4-e5f6-7890-abcd-1234567890ab").
        /// Passed as route parameter in URLs (e.g., /Roles/Details/abc123).
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// The display name of the role.
        /// Maps to AspNetRoles.Name in the database.
        /// Examples: "Admin", "Manager", "User", "Supervisor", "Auditor", "Guest"
        /// 
        /// Displayed prominently at the top of the details page as the page title.
        /// Used to identify the role in the UI and audit logs.
        /// 
        /// Special Roles:
        /// - "Admin": Typically shown with a badge or special styling to indicate privileged status
        /// - System may prevent deletion of critical roles like "Admin"
        /// 
        /// Naming Conventions:
        /// - Use PascalCase (e.g., "ProductManager", not "product_manager")
        /// - Keep names concise but descriptive
        /// - Avoid special characters (stick to letters and spaces)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The total number of users currently assigned to this role.
        /// Calculated by counting entries in AspNetUserRoles table for this role.
        /// 
        /// Display Purpose:
        /// Shown as a summary statistic: "10 users have this role"
        /// 
        /// Use Cases:
        /// - Impact Assessment: Before modifying/deleting role, see how many users are affected
        /// - Role Popularity: Identify frequently used vs. rarely used roles
        /// - Capacity Planning: Understand role distribution in the system
        /// 
        /// Delete Warning:
        /// When UserCount > 0, display warning:
        /// "⚠️ This role is assigned to {UserCount} user(s). 
        /// Deleting this role will remove it from all users."
        /// 
        /// Empty Role (UserCount = 0):
        /// Safe to delete without affecting any users.
        /// May display: "No users currently have this role."
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// List of email addresses of users who have this role assigned.
        /// Used to display a table or list of role members on the details page.
        /// 
        /// Data Source:
        /// Populated by joining AspNetUserRoles with AspNetUsers table:
        /// SELECT u.Email FROM AspNetUsers u
        /// JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        /// WHERE ur.RoleId = @RoleId
        /// 
        /// Display Format:
        /// Typically rendered as a table with:
        /// - User email (clickable link to user details)
        /// - "Edit User" button to modify that user
        /// - "Remove from Role" action (optional)
        /// 
        /// Empty State:
        /// If list is empty, display: "No users have this role yet."
        /// 
        /// Use Cases:
        /// - Auditing: See who has access to role-based permissions
        /// - Role Management: Identify users to contact before role changes
        /// - Troubleshooting: Verify specific user has the expected role
        /// - Reporting: Generate user-role assignment reports
        /// 
        /// Performance Note:
        /// For roles with hundreds of users, consider pagination or showing only first N users
        /// with a "View All" link to prevent page slowdown.
        /// </summary>
        public List<string> Users { get; set; } = new();

        /// <summary>
        /// List of claim types (permission names) assigned to this role.
        /// Shows what permissions ALL users in this role automatically inherit.
        /// 
        /// Data Source:
        /// Populated from AspNetRoleClaims table:
        /// SELECT ClaimType FROM AspNetRoleClaims
        /// WHERE RoleId = @RoleId AND ClaimValue = 'true'
        /// 
        /// Example Values:
        /// ["CanEditUsers", "CanViewRoles", "CanManageClaims", "CanViewAuditLogs"]
        /// 
        /// Display Format:
        /// Typically rendered as:
        /// - Badge list: Each claim shown as a colored badge
        /// - Table: Claim name with description from ClaimDefinitionsService
        /// - Checkboxes: Read-only view of assigned claims
        /// 
        /// Inheritance Indicator:
        /// Display note: "All users with this role inherit these permissions automatically."
        /// 
        /// Navigation:
        /// Include "Manage Role Claims" button linking to /RoleClaims/Manage/{Id}
        /// where admin can add/remove claims for this role.
        /// 
        /// Empty State:
        /// If list is empty, display: 
        /// "No claims assigned to this role. Users in this role have no special permissions."
        /// Show "Assign Claims" button to get started.
        /// 
        /// Special Case - Admin Role:
        /// If Name == "Admin", display all available claims (from ClaimDefinitionsService)
        /// with note: "Admin role has full system access regardless of assigned claims."
        /// 
        /// Use Cases:
        /// - Permission Auditing: Review what a role can do
        /// - Compliance: Generate reports of role capabilities
        /// - Troubleshooting: Verify role has expected permissions
        /// - Impact Analysis: See what permissions users lose if role is deleted
        /// 
        /// Performance Note:
        /// Claims list is typically small (< 20 items) so no pagination needed.
        /// Descriptions can be loaded from ClaimDefinitionsService for user-friendly display.
        /// </summary>
        public List<string> Claims { get; set; } = new();
    }
}