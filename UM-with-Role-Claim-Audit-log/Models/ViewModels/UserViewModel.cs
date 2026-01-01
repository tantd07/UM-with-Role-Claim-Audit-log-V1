namespace UM_with_Role_Claim_Audit_log.Models.ViewModels
{
    /// <summary>
    /// View model for displaying user information in list views and user details pages.
    /// Used in multiple contexts:
    /// - Users/Index: Display all users in a table with roles, status, and actions
    /// - Users/Details: Show comprehensive user information with roles and claims
    /// - Dashboard views: User statistics and summaries
    /// 
    /// This ViewModel provides a complete snapshot of a user's identity, security status,
    /// assigned roles, and direct claims. It combines data from multiple Identity tables:
    /// - AspNetUsers (Id, Email, UserName, EmailConfirmed, LockoutEnabled, LockoutEnd)
    /// - AspNetUserRoles (via Roles list)
    /// - AspNetUserClaims (via Claims list)
    /// 
    /// Key Features:
    /// - Account status tracking (active/locked via IsActive computed property)
    /// - Role membership display (all assigned roles)
    /// - Direct claims display (user-specific permissions, excludes inherited role claims)
    /// - Email confirmation status
    /// - Lockout information for security monitoring
    /// 
    /// Design Pattern:
    /// Read-only ViewModel - used for displaying data, not for editing.
    /// For user modification, use EditUserViewModel, CreateUserViewModel, or ResetPasswordViewModel.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// The unique identifier of the user.
        /// Maps to AspNetUsers.Id in the database.
        /// 
        /// Usage:
        /// - Generate action links (Edit, Delete, Details, ResetPassword, ManageClaims)
        /// - Route parameter for user-specific pages (e.g., /Users/Details/abc123)
        /// - Foreign key for user-related operations
        /// 
        /// Format:
        /// Typically a GUID string (e.g., "a1b2c3d4-e5f6-7890-abcd-1234567890ab")
        /// Generated automatically by ASP.NET Core Identity when user is created.
        /// 
        /// Not displayed to end users in the UI - used internally for identification and routing.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the user.
        /// Maps to AspNetUsers.Email in the database.
        /// 
        /// Purpose:
        /// - Primary identifier displayed in user lists and details
        /// - Used for login (email = username in this system)
        /// - Contact information for password reset and notifications (v2.0 feature)
        /// 
        /// Display Context:
        /// - In list view: Main column, clickable link to user details
        /// - In details view: Header showing "User Details for: user@example.com"
        /// - In audit logs: Identifies which user performed actions
        /// 
        /// Security Note:
        /// Displayed to admins only - regular users cannot see other users' emails
        /// (requires CanViewUsers or CanEditUsers claim to access user list).
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The username used for authentication.
        /// Maps to AspNetUsers.UserName in the database.
        /// 
        /// In This System:
        /// UserName is set to the same value as Email for simplicity.
        /// Users login with their email address, not a separate username.
        /// 
        /// Why Email = Username:
        /// - Easier for users to remember (one credential instead of two)
        /// - Common pattern in modern web applications
        /// - Reduces registration friction
        /// - Simplifies password recovery (email is both identifier and recovery method)
        /// 
        /// Alternative Approach (for Customization):
        /// Buyers can modify to use separate usernames if needed:
        /// - CreateUserViewModel: Add separate UserName input field
        /// - SeedData.cs: Set UserName differently from Email
        /// - Login page: Accept UserName instead of Email
        /// 
        /// Currently NOT displayed in UI (since it duplicates Email).
        /// Kept for Identity framework compatibility and future extensibility.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the user's email address has been confirmed.
        /// Maps to AspNetUsers.EmailConfirmed in the database.
        /// 
        /// Purpose:
        /// - Verify user owns the email address they registered with
        /// - Security measure to prevent unauthorized account creation
        /// - Can gate certain features behind email confirmation
        /// 
        /// v1.0 Behavior:
        /// Email confirmation is NOT enforced (RequireConfirmedAccount = false in Program.cs).
        /// Admin can manually set this to true when creating users via CreateUserViewModel.
        /// Default: true for admin-created accounts, allowing immediate login.
        /// 
        /// v2.0 Planned Behavior:
        /// Will integrate with email verification workflow:
        /// - User registers → Email sent with confirmation link
        /// - User clicks link → EmailConfirmed = true
        /// - Only confirmed users can login
        /// 
        /// Display in UI:
        /// - True: Green badge "✓ Verified" or "Email Confirmed"
        /// - False: Yellow badge "⚠ Unverified" or "Email Not Confirmed"
        /// - Admin can toggle this value in Users/Edit page
        /// 
        /// Use Cases:
        /// - Account cleanup: Identify users who never confirmed email
        /// - Security audit: Ensure all active users have confirmed emails
        /// - Troubleshooting: User can't login → Check if EmailConfirmed = false
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Indicates whether account lockout is enabled for this user.
        /// Maps to AspNetUsers.LockoutEnabled in the database.
        /// 
        /// Purpose:
        /// Controls whether the user account can be temporarily locked for security reasons.
        /// When enabled, the account can be locked out after too many failed login attempts
        /// or by admin action (setting LockoutEnd to a future date).
        /// 
        /// Typical Values:
        /// - True (default): User account can be locked out (security feature enabled)
        /// - False: User account cannot be locked out (bypass security feature)
        /// 
        /// Use Cases:
        /// 
        /// LockoutEnabled = True:
        /// - Standard user accounts (can be locked for security)
        /// - Failed login attempts trigger automatic lockout
        /// - Admin can manually lock account by setting LockoutEnd
        /// 
        /// LockoutEnabled = False:
        /// - Service accounts that should never be locked
        /// - Critical admin accounts (use with caution!)
        /// - Testing/development accounts
        /// 
        /// Security Consideration:
        /// Setting to false disables an important security feature.
        /// Only disable for trusted accounts (e.g., system service accounts).
        /// Most user accounts should have LockoutEnabled = true.
        /// 
        /// Combined with LockoutEnd:
        /// Account is locked if: LockoutEnabled = true AND LockoutEnd > now
        /// See IsActive property for the complete lockout logic.
        /// 
        /// Display in UI:
        /// Typically not shown directly - instead show account status via IsActive property.
        /// Advanced admin view may show: "Lockout Feature: Enabled/Disabled"
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// The date and time when the user's account lockout expires.
        /// Maps to AspNetUsers.LockoutEnd in the database.
        /// 
        /// Purpose:
        /// Controls temporary account suspension for security or administrative reasons.
        /// When set to a future date, the user cannot login until that time passes.
        /// 
        /// Values & Meaning:
        /// - null: Account is NOT locked (normal state)
        /// - Past date: Account WAS locked but lockout has expired (now unlocked)
        /// - Future date: Account IS currently locked (user cannot login)
        /// 
        /// How Lockout Works:
        /// 
        /// Automatic Lockout (Security):
        /// 1. User enters wrong password 5 times (configurable in Program.cs)
        /// 2. Identity automatically sets LockoutEnd = DateTimeOffset.Now + 15 minutes
        /// 3. User cannot login for 15 minutes
        /// 4. After 15 minutes, LockoutEnd < Now → account automatically unlocks
        /// 
        /// Manual Lockout (Admin Action):
        /// 1. Admin suspects account compromise or wants to disable account
        /// 2. Admin sets LockoutEnd = DateTimeOffset.Now + 30 days (or indefinite)
        /// 3. User cannot login until admin removes lockout or time expires
        /// 
        /// Unlock Account:
        /// Admin can unlock by setting LockoutEnd = null or a past date.
        /// 
        /// Display in UI:
        /// - null: "Active" (green badge)
        /// - Past: "Previously Locked (Now Active)" (blue info)
        /// - Future: "Locked until {date}" (red badge)
        /// 
        /// Example Displays:
        /// - "Locked until 2025-01-15 10:30 AM" (still locked)
        /// - "Unlocks in 2 hours 15 minutes" (countdown)
        /// - "Account is active" (null or past date)
        /// 
        /// Use Cases:
        /// - Security: Prevent brute-force login attacks
        /// - Admin suspension: Temporarily disable problematic users
        /// - Investigation: Lock account during security review
        /// - Compliance: Enforce cooling-off periods
        /// 
        /// Audit Logging:
        /// Lockout changes should be logged:
        /// - "User account locked by admin until {date}"
        /// - "User account unlocked by admin"
        /// - "Account automatically locked due to failed login attempts"
        /// 
        /// Type: DateTimeOffset? (nullable)
        /// - Nullable allows representing "no lockout" state (null)
        /// - DateTimeOffset (not DateTime) provides timezone information
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// List of role names assigned to this user.
        /// Populated from AspNetUserRoles table joined with AspNetRoles.
        /// 
        /// Purpose:
        /// Shows which roles the user belongs to, determining their role-based permissions.
        /// Roles grant access to features and enable claim inheritance.
        /// 
        /// Data Source:
        /// Retrieved via UserManager.GetRolesAsync(user):
        /// SELECT r.Name FROM AspNetRoles r
        /// JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
        /// WHERE ur.UserId = @UserId
        /// 
        /// Example Values:
        /// - ["Admin"] - User has full system access
        /// - ["Manager", "User"] - User has multiple roles (permissions are combined)
        /// - [] - Empty list (user has no roles, only direct claims)
        /// 
        /// Display Format:
        /// Typically rendered as badges in the UI:
        /// - Admin role: Red badge "Admin"
        /// - Manager role: Blue badge "Manager"
        /// - User role: Gray badge "User"
        /// - Multiple roles: Multiple badges side-by-side
        /// 
        /// Role-Based Claims Inheritance:
        /// Users automatically inherit ALL claims assigned to their roles.
        /// Example:
        /// - Manager role has claims: ["CanEditUsers", "CanViewRoles"]
        /// - User with Manager role inherits these claims automatically
        /// - Claims inherited from roles are shown as "From Role: Manager" in Claims UI
        /// 
        /// Empty State:
        /// If list is empty: Display "No roles assigned" or "-"
        /// User may still have direct claims (non-role-based permissions).
        /// 
        /// Use Cases:
        /// - User list view: Quick identification of user privileges
        /// - User details: Show role memberships
        /// - Role assignment: Pre-select checkboxes for current roles
        /// - Troubleshooting: Verify user has expected role
        /// - Audit reports: Track role assignments across users
        /// 
        /// Navigation:
        /// Clicking role name in UI could navigate to /Roles/Details/{roleId}
        /// to see what claims that role provides.
        /// 
        /// Management:
        /// Modified via Users/Edit page using AssignRoleViewModel.
        /// Changes stored in AspNetUserRoles table (many-to-many relationship).
        /// </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// List of claim types (permission names) directly assigned to this user.
        /// Populated from AspNetUserClaims table (user-specific claims only).
        /// 
        /// Important: This list contains ONLY direct user claims, NOT inherited role claims.
        /// For complete permission evaluation, the system checks both direct and inherited claims.
        /// 
        /// Purpose:
        /// Shows user-specific permissions that override or supplement role-based permissions.
        /// Allows granting exceptions or additional permissions to individual users.
        /// 
        /// Data Source:
        /// Retrieved via UserManager.GetClaimsAsync(user):
        /// SELECT ClaimType FROM AspNetUserClaims
        /// WHERE UserId = @UserId AND ClaimValue = 'true'
        /// 
        /// Example Values:
        /// - ["CanEditUsers", "CanViewAuditLogs"] - User has these direct permissions
        /// - [] - Empty list (user only has role-based claims)
        /// 
        /// Display Format:
        /// Typically rendered as badges with indicators:
        /// - Direct claim: Green badge "CanEditUsers" with icon "👤 Direct"
        /// - Inherited claim: Blue badge "CanViewRoles" with icon "🛡️ From Role: Manager"
        /// 
        /// Claims Management UI shows both types:
        /// - Direct claims: Checkboxes enabled (admin can add/remove)
        /// - Inherited claims: Checkboxes checked but disabled (remove via role management)
        /// 
        /// Direct Claims vs Inherited Claims:
        /// 
        /// Direct Claims (this list):
        /// - Stored in AspNetUserClaims table
        /// - User-specific exceptions or additions
        /// - Managed via /Claims/Manage/{userId}
        /// - Example: Give "CanDeleteUsers" to one user without changing their role
        /// 
        /// Inherited Claims (NOT in this list):
        /// - Stored in AspNetRoleClaims table
        /// - Granted via role membership
        /// - Managed via /RoleClaims/Manage/{roleId}
        /// - Example: All Managers inherit "CanEditUsers" from Manager role
        /// 
        /// Authorization Evaluation:
        /// ClaimBasedAuthorizationHandler checks BOTH:
        /// 1. Does user have direct claim? (this list)
        /// 2. Does user's role have this claim? (inherited)
        /// If either is true → Access granted
        /// 
        /// Empty State:
        /// If list is empty: Display "No direct claims" or "-"
        /// User may still have inherited claims from roles.
        /// 
        /// Use Cases:
        /// - User list view: Show user-specific permissions at a glance
        /// - User details: Display both direct and inherited claims
        /// - Permission audit: Identify users with exceptional permissions
        /// - Troubleshooting: Verify user has expected permissions
        /// - Compliance: Generate reports of direct permission assignments
        /// 
        /// Performance Note:
        /// Claims list is typically small (< 10 items) per user.
        /// For complete permission view, inherited claims are loaded separately
        /// in the ManageClaimsViewModel when needed.
        /// 
        /// Security Best Practice:
        /// Prefer role-based claims over direct claims for easier management.
        /// Use direct claims only for exceptional cases or user-specific needs.
        /// </summary>
        public List<string> Claims { get; set; } = new();

        /// <summary>
        /// Computed property that determines if the user account is currently active.
        /// Combines lockout settings (LockoutEnabled and LockoutEnd) to determine account status.
        /// 
        /// Logic:
        /// Account is active (user can login) if ANY of these conditions are true:
        /// 1. LockoutEnabled = false (lockout feature disabled, account always active)
        /// 2. LockoutEnd = null (no lockout set, account is active)
        /// 3. LockoutEnd <= DateTimeOffset.Now (lockout expired, account unlocked)
        /// 
        /// Account is locked (user cannot login) if ALL of these conditions are true:
        /// 1. LockoutEnabled = true (lockout feature enabled)
        /// 2. LockoutEnd != null (lockout has been set)
        /// 3. LockoutEnd > DateTimeOffset.Now (lockout still in effect)
        /// 
        /// Truth Table:
        /// 
        /// | LockoutEnabled | LockoutEnd      | IsActive | Status              |
        /// |---------------|-----------------|----------|---------------------|
        /// | false         | (any)           | true     | Always active       |
        /// | true          | null            | true     | Active (no lockout) |
        /// | true          | past date       | true     | Active (expired)    |
        /// | true          | future date     | false    | LOCKED              |
        /// 
        /// Display in UI:
        /// Use this property to show account status with visual indicators:
        /// 
        /// IsActive = true:
        /// - Badge: Green "✓ Active" or "Online"
        /// - Allow: Edit, Reset Password, Manage Claims actions
        /// - Login: User can login normally
        /// 
        /// IsActive = false:
        /// - Badge: Red "🔒 Locked" or "Suspended"
        /// - Show: "Locked until {LockoutEnd.Value:yyyy-MM-dd HH:mm}"
        /// - Action: "Unlock Account" button for admin
        /// - Login: User sees "Account is locked" error
        /// 
        /// Use Cases:
        /// 
        /// 1. User List View:
        ///    - Filter: "Show only active users" / "Show locked users"
        ///    - Column: Status badge (Active/Locked)
        ///    - Highlight: Red background for locked accounts
        /// 
        /// 2. User Details:
        ///    - Header: "Account Status: Active" (green) or "Locked" (red)
        ///    - Details: Show LockoutEnd if locked
        ///    - Action: Enable/disable management actions based on status
        /// 
        /// 3. Security Dashboard:
        ///    - Statistics: "X active users, Y locked users"
        ///    - Alerts: "5 accounts locked in last 24 hours"
        /// 
        /// 4. Bulk Operations:
        ///    - "Unlock all expired accounts" → Set LockoutEnd = null for all
        ///    - "Lock all inactive users" → Set LockoutEnd for dormant accounts
        /// 
        /// Real-Time Evaluation:
        /// This property is NOT stored in database - calculated every time it's accessed.
        /// Lockout status automatically changes when current time passes LockoutEnd.
        /// No background job needed to unlock accounts - happens at next access.
        /// 
        /// Example Scenarios:
        /// 
        /// Scenario 1: Failed Login Lockout
        /// - User enters wrong password 5 times at 10:00 AM
        /// - System sets: LockoutEnabled = true, LockoutEnd = 10:15 AM
        /// - 10:10 AM: IsActive = false (still locked)
        /// - 10:16 AM: IsActive = true (lockout expired, auto-unlocked)
        /// 
        /// Scenario 2: Admin Manual Lock
        /// - Admin locks account at 10:00 AM for investigation
        /// - Sets: LockoutEnabled = true, LockoutEnd = 10:00 PM (12 hours)
        /// - All day: IsActive = false
        /// - 10:01 PM: IsActive = true (lockout expired)
        /// 
        /// Scenario 3: Permanent Disable
        /// - Admin wants to permanently disable account
        /// - Sets: LockoutEnabled = true, LockoutEnd = DateTimeOffset.MaxValue (year 9999)
        /// - Forever: IsActive = false (effectively permanent ban)
        /// 
        /// Performance:
        /// Very fast - simple boolean comparison, no database query.
        /// Safe to call frequently in UI rendering.
        /// 
        /// Alternative Implementation Note:
        /// Some systems add a separate "IsActive" or "IsDisabled" boolean column.
        /// This implementation uses existing Identity lockout features, avoiding custom fields.
        /// </summary>
        public bool IsActive => !LockoutEnabled || LockoutEnd == null || LockoutEnd <= DateTimeOffset.Now;
    }
}