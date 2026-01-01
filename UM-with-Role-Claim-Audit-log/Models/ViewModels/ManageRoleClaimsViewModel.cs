namespace UM_with_Role_Claim_Audit_log.Models.ViewModels
{
    /// <summary>
    /// View model for managing claims (permissions) assigned to roles.
    /// Used in the RoleClaims/Manage page to assign permissions that will be inherited by all users in that role.
    /// 
    /// Key Concept: Role-Based Claims Inheritance
    /// When a claim is assigned to a role, ALL users who have that role automatically inherit the permission.
    /// This provides centralized permission management - change role claims once, affects all role members.
    /// 
    /// Special Handling for Admin Role:
    /// - Admin role claims are auto-selected and displayed as enabled (read-only in UI)
    /// - Admin role bypasses all claim checks via ClaimBasedAuthorizationHandler
    /// - UI prevents modification of Admin role claims to maintain system security
    /// 
    /// Database Impact:
    /// - Role claims stored in AspNetRoleClaims table
    /// - Users inherit these claims at runtime (not duplicated in AspNetUserClaims)
    /// - ClaimBasedAuthorizationHandler evaluates both direct user claims and inherited role claims
    /// </summary>
    public class ManageRoleClaimsViewModel
    {
        /// <summary>
        /// The unique identifier of the role whose claims are being managed.
        /// Maps to AspNetRoles.Id in the database.
        /// Used to identify which role's permissions are being modified.
        /// Passed as route parameter (e.g., /RoleClaims/Manage/abc123).
        /// </summary>
        public string RoleId { get; set; } = string.Empty;

        /// <summary>
        /// The name of the role for display purposes.
        /// Examples: "Admin", "Manager", "User", "Supervisor", "Auditor"
        /// Maps to AspNetRoles.Name in the database.
        /// 
        /// Shown prominently in the UI header to confirm which role is being modified.
        /// Critical for preventing accidental permission assignments to wrong role.
        /// 
        /// Special Case - Admin Role:
        /// When RoleName = "Admin", the UI shows a warning banner:
        /// "Admin role has full system access. All claims are automatically enabled."
        /// This prevents confusion about why checkboxes appear enabled but disabled.
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Collection of all available claims in the system with their assignment status for this role.
        /// Reuses UserClaimViewModel for consistency across the application (same structure as user claims).
        /// 
        /// Each claim includes:
        /// - ClaimType: Permission identifier (e.g., "CanEditUsers")
        /// - ClaimValue: Typically "true" for permission claims
        /// - Description: Human-readable explanation from ClaimDefinitionsService
        /// - IsSelected: Whether this role currently has this permission
        /// - IsInheritedFromRole: Not used in role context (always false for role claims)
        /// - IsReadOnly: Set to true for Admin role claims (prevents modification)
        /// 
        /// Data Flow:
        /// 1. Load: Claims populated from ClaimDefinitionsService.GetAllClaims()
        /// 2. Display: Each claim shows as checkbox with description
        /// 3. Special handling: Admin role claims are pre-selected and read-only
        /// 4. Submit: Selected claims saved to AspNetRoleClaims table
        /// 5. Effect: All users in this role immediately inherit these permissions
        /// 
        /// Important Notes:
        /// - Claims are NOT copied to AspNetUserClaims (inheritance is runtime evaluation)
        /// - ClaimBasedAuthorizationHandler checks both user claims and role claims
        /// - Removing a claim from a role removes it from ALL users in that role
        /// - Adding a claim to a role grants it to ALL users in that role
        /// 
        /// Admin Role Special Behavior:
        /// - All claims automatically selected (IsSelected = true)
        /// - All checkboxes disabled (IsReadOnly = true)
        /// - Admin bypasses claim checks in ClaimBasedAuthorizationHandler anyway
        /// - UI prevents accidental removal of Admin permissions
        /// </summary>
        public List<UserClaimViewModel> Claims { get; set; } = new();
    }
}