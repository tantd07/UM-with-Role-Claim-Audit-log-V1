namespace UM_with_Role_Claim_Audit_log.Models.ViewModels
{
    /// <summary>
    /// View model for managing claims (permissions) assigned to individual users.
    /// Used in the Claims/Manage page to display and modify user-specific permissions.
    /// Supports both direct user claims and inherited role-based claims.
    /// Key feature: Visual distinction between direct claims and inherited claims from roles.
    /// </summary>
    public class ManageClaimsViewModel
    {
        /// <summary>
        /// The unique identifier of the user whose claims are being managed.
        /// Maps to AspNetUsers.Id in the database.
        /// Used to identify which user's permissions are being modified.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the user for display purposes.
        /// Shown in the UI header to confirm which user's claims are being managed.
        /// Provides context to prevent accidental permission changes to wrong user.
        /// </summary>
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>
        /// Collection of all available claims in the system with their assignment status.
        /// Each claim is represented by a UserClaimViewModel with:
        /// - Selection status (checkbox state)
        /// - Description from ClaimDefinitionsService
        /// - Inheritance information (from role or direct)
        /// - Read-only flag (prevents removing inherited claims)
        /// 
        /// Claims are loaded from ClaimDefinitionsService.GetAllClaims() which serves
        /// as the single source of truth for all permissions in the system.
        /// </summary>
        public List<UserClaimViewModel> Claims { get; set; } = new();
    }

    /// <summary>
    /// Represents a single claim (permission) with its assignment status and metadata.
    /// Used within ManageClaimsViewModel to display checkboxes with additional context.
    /// 
    /// Key Features:
    /// - Visual indicators for inherited vs. direct claims
    /// - Read-only mode for role-based claims (cannot be unchecked)
    /// - Descriptive text from ClaimDefinitionsService for user-friendly display
    /// 
    /// This design supports the hybrid authorization model where users get permissions from:
    /// 1. Direct user claims (stored in AspNetUserClaims)
    /// 2. Role-based claims (inherited from AspNetRoleClaims via user's roles)
    /// </summary>
    public class UserClaimViewModel
    {
        /// <summary>
        /// The type identifier of the claim (permission name).
        /// Examples: "CanEditUsers", "CanDeleteRoles", "CanViewAuditLogs"
        /// Maps to ClaimType column in AspNetUserClaims and AspNetRoleClaims tables.
        /// Must match keys in ClaimDefinitionsService.AvailableClaims dictionary.
        /// Used by the DynamicPolicyProvider to generate authorization policies at runtime.
        /// </summary>
        public string ClaimType { get; set; } = string.Empty;

        /// <summary>
        /// The value of the claim, typically "true" for permission claims.
        /// For permission-based authorization, value is always "true" to indicate "has permission".
        /// Maps to ClaimValue column in AspNetUserClaims and AspNetRoleClaims tables.
        /// The ClaimBasedAuthorizationHandler checks for this exact value during authorization.
        /// </summary>
        public string ClaimValue { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable description of what this claim/permission allows.
        /// Loaded from ClaimDefinitionsService.AvailableClaims dictionary.
        /// Examples:
        /// - "CanEditUsers" → "Can create and modify user accounts"
        /// - "CanDeleteRoles" → "Can delete roles from the system"
        /// 
        /// Displayed in the UI to help admins understand what each permission grants.
        /// This centralizes claim descriptions in one place (ClaimDefinitionsService),
        /// supporting the single source of truth principle.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether this claim is currently assigned to the user.
        /// True = checkbox is checked (user has this permission)
        /// False = checkbox is unchecked (user does not have this permission)
        /// 
        /// For inherited claims (IsInheritedFromRole = true):
        /// - Always shows as checked
        /// - Checkbox is disabled (IsReadOnly = true)
        /// - Cannot be removed via this interface (must remove from role or remove role from user)
        /// 
        /// For direct user claims:
        /// - Can be checked/unchecked freely
        /// - Changes saved to AspNetUserClaims table
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Indicates whether this claim comes from one of the user's assigned roles.
        /// True = User has this claim because they belong to a role that has this claim
        /// False = This is a direct user claim (or user doesn't have this claim)
        /// 
        /// How Role Claims Work:
        /// 1. Admin assigns claim to Role (e.g., Manager role gets "CanEditUsers")
        /// 2. User is assigned to that Role
        /// 3. User automatically inherits all claims from that Role
        /// 4. ClaimBasedAuthorizationHandler evaluates both direct and inherited claims
        /// 
        /// Visual UI Indicators:
        /// - Inherited claims show with a badge/icon: "From Role: Manager"
        /// - Checkbox is disabled (read-only) to prevent confusion
        /// - User must remove role or modify role claims to change permission
        /// 
        /// This supports the principle: "Role-based claims affect all users in that role"
        /// </summary>
        public bool IsInheritedFromRole { get; set; } // Claim from Role

        /// <summary>
        /// Indicates whether this claim checkbox should be disabled (read-only) in the UI.
        /// True = Checkbox is disabled (typically for inherited claims)
        /// False = Checkbox is enabled (user can check/uncheck)
        /// 
        /// Use Cases:
        /// - Inherited role claims: IsReadOnly = true (prevents accidental removal attempts)
        /// - Direct user claims: IsReadOnly = false (admin can freely assign/remove)
        /// - Special system claims: Could be set to true for protection (future feature)
        /// 
        /// UI Behavior:
        /// - Read-only checkboxes are visually distinct (grayed out)
        /// - Tooltip explains: "This permission comes from the [RoleName] role"
        /// - Admin must use /RoleClaims/Manage or /Users/Edit to change inherited claims
        /// 
        /// Security Benefit:
        /// Prevents admins from accidentally thinking they removed a permission
        /// when it's still active via role inheritance.
        /// </summary>
        public bool IsReadOnly { get; set; } // Do not uncheck if it is from a role
    }
}