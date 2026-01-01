using System.ComponentModel.DataAnnotations;

namespace UM_with_Role_Claim_Audit_log.Models.ViewModels
{
    /// <summary>
    /// View model for creating new user accounts via the admin interface.
    /// Used in the Users/Create page to collect user registration data with validation.
    /// Includes built-in validation attributes for server-side and client-side validation.
    /// Admin can create users with pre-confirmed email addresses (v1.0 feature).
    /// </summary>
    public class CreateUserViewModel
    {
        /// <summary>
        /// The email address for the new user account.
        /// Serves as both the username and primary contact method.
        /// Must be a valid email format and is required.
        /// This will be stored in AspNetUsers.Email and AspNetUsers.UserName.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The password for the new user account.
        /// Must meet the password policy requirements configured in Program.cs:
        /// - Minimum 6 characters (configurable via StringLength attribute)
        /// - Maximum 100 characters
        /// - Default policy: requires uppercase, lowercase, digit, and special character
        /// Password is hashed before storage using ASP.NET Core Identity.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation field to prevent typos during user creation.
        /// Must exactly match the Password field.
        /// Validation occurs both client-side (via JavaScript) and server-side (via Compare attribute).
        /// Not stored in database - only used for validation during account creation.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the user's email should be marked as confirmed upon creation.
        /// Default value: true (v1.0 feature - bypasses email confirmation process).
        /// 
        /// When true: User can login immediately without email verification
        /// When false: User would need to confirm email before login (v2.0 feature)
        /// 
        /// This allows admins to create ready-to-use accounts for employees or team members.
        /// Maps to AspNetUsers.EmailConfirmed in the database.
        /// </summary>
        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; } = true; // Default: true for immediate access
    }
}