namespace UM_with_Role_Claim_Audit_log.Services
{
    /// <summary>
    /// Centralized service for managing claim definitions and descriptions.
    /// Single source of truth for all available claims in the system.
    /// </summary>
    public class ClaimDefinitionsService
    {
        /// <summary>
        /// Dictionary containing all available claims in the system.
        /// Key: Claim type (e.g., "CanEditUsers")
        /// Value: Human-readable description
        /// </summary>
        public static readonly Dictionary<string, string> AvailableClaims = new()
        {
            // User Management Claims
            { "CanEditUsers", "Can create and modify user accounts" },
            { "CanDeleteUsers", "Can delete user accounts" },
            { "CanResetPasswords", "Can reset user passwords" },
            { "CanDisableUsers", "Can enable/disable user accounts" },
            { "CanViewUsers", "Can view user accounts (read-only)"},
            
            // Role Management Claims
            { "CanViewRoles", "Can view roles (read-only)" },
            { "CanManageRoles", "Can create and manage roles" },
            { "CanCreateRoles", "Can create new roles" },
            { "CanDeleteRoles", "Can delete roles" },
            
            // Claims Management Claims
            { "CanViewClaims", "Can view claims (read-only)" },
            { "CanManageClaims", "Can assign claims to users" },
            { "CanAssignClaims", "Can assign claims to users" },
            { "CanManageRoleClaims", "Can manage claims for roles" },
            
            // Audit & Security Claims
            { "CanViewAuditLogs", "Can view system audit logs" },
            { "ViewSensitiveData", "Can view sensitive user information" },
            
            // 🆕 ADD A NEW CLAIM ONLY HERE - AUTOMATIC UI SYNC!
            // { "CanApproveInvoices", "Can approve pending invoices" },
            // { "CanExportReports", "Can export system reports to Excel/PDF" },
            // { "CanAccessFinancialData", "Can view financial data and analytics" },
        };

        /// <summary>
        /// Gets the description for a specific claim type.
        /// </summary>
        /// <param name="claimType">The claim type to get description for</param>
        /// <returns>Description if found, "Custom permission" otherwise</returns>
        public static string GetClaimDescription(string claimType)
        {
            return AvailableClaims.TryGetValue(claimType, out var description)
                ? description
                : "Custom permission";
        }

        /// <summary>
        /// Gets all available claims as a list of key-value pairs.
        /// </summary>
        public static IEnumerable<KeyValuePair<string, string>> GetAllClaims()
        {
            return AvailableClaims;
        }

        /// <summary>
        /// Checks if a claim type is defined in the system.
        /// </summary>
        public static bool IsValidClaim(string claimType)
        {
            return AvailableClaims.ContainsKey(claimType);
        }
    }
}