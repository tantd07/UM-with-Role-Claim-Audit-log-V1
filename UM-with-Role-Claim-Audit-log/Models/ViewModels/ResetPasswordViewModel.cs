using System.ComponentModel.DataAnnotations;

namespace UM_with_Role_Claim_Audit_log.Models.ViewModels
{
    /// <summary>
    /// View model for admin-initiated password reset functionality.
    /// Used in the Users/ResetPassword page to allow admins to change user passwords without requiring the old password.
    /// This is an administrative override feature - does NOT require email verification or password reset tokens.
    /// 
    /// Security Note:
    /// This functionality is restricted to users with CanResetPasswords claim (typically admins only).
    /// Allows helpdesk/admin staff to reset passwords for locked-out or forgotten password scenarios.
    /// All password resets are logged to the audit trail for security compliance.
    /// 
    /// Separation from EditUserViewModel:
    /// Password management is intentionally separated from user editing for security reasons:
    /// - Prevents accidental password changes during routine user edits
    /// - Provides dedicated UI for password operations
    /// - Enables separate audit logging for password changes
    /// - Follows principle of least privilege (different permissions for edit vs password reset)
    /// </summary>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// The unique identifier of the user whose password is being reset.
        /// Maps to AspNetUsers.Id in the database.
        /// Used to locate the correct user account when applying the new password.
        /// Typically passed as a route parameter (e.g., /Users/ResetPassword/abc123).
        /// This field is not displayed in the form (hidden input) but is essential for processing.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the user whose password is being reset.
        /// Used for display purposes in the UI to confirm the correct user.
        /// Shows as read-only text in the form: "Reset password for: user@example.com"
        /// 
        /// Security Benefit:
        /// Provides visual confirmation to prevent accidentally resetting the wrong user's password.
        /// Admin sees: "Are you sure you want to reset password for: john.doe@company.com?"
        /// This reduces the risk of administrative errors.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The new password to be assigned to the user account.
        /// Must meet the password policy requirements configured in Program.cs:
        /// - Minimum 6 characters (configurable via StringLength attribute)
        /// - Maximum 100 characters
        /// - Default policy: requires uppercase, lowercase, digit, and special character
        /// 
        /// Password is hashed using ASP.NET Core Identity's password hasher before storage.
        /// The plain-text password is never stored in the database.
        /// 
        /// Admin Override:
        /// Admin can set any password that meets policy requirements without:
        /// - Knowing the user's current password
        /// - Requiring email verification
        /// - Using password reset tokens
        /// 
        /// This is a privileged operation logged to the audit trail with admin's user ID.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation field to prevent typos when setting new passwords.
        /// Must exactly match the NewPassword field.
        /// Validation occurs both client-side (via JavaScript) and server-side (via Compare attribute).
        /// Not stored in database - only used for validation during password reset operation.
        /// 
        /// User Experience:
        /// - Prevents admin from accidentally setting a mistyped password
        /// - User won't be locked out due to admin typo
        /// - Immediate feedback if passwords don't match
        /// 
        /// Security Best Practice:
        /// Password confirmation is standard practice for any password-setting operation
        /// to ensure accuracy and prevent user lockouts from administrative errors.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}