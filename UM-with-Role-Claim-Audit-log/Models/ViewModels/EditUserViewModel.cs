using System.ComponentModel.DataAnnotations;

namespace UM_with_Role_Claim_Audit_log.Models.ViewModels
{
    /// <summary>
    /// View model for editing existing user accounts via the admin interface.
    /// Used in the Users/Edit page to modify user details and role assignments.
    /// Supports changing email, email confirmation status, and role memberships.
    /// Does NOT include password changes - use ResetPasswordViewModel for that purpose.
    /// </summary>
    public class EditUserViewModel
    {
        /// <summary>
        /// The unique identifier of the user being edited.
        /// Maps to AspNetUsers.Id in the database.
        /// Used to locate the correct user record when saving changes.
        /// This value is typically passed as a route parameter (e.g., /Users/Edit/abc123).
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the user.
        /// Can be modified by admin to update the user's login credentials.
        /// Must be a valid email format and is required.
        /// Maps to both AspNetUsers.Email and AspNetUsers.UserName (email is used as username).
        /// Note: Changing email may require re-confirmation in v2.0 when email verification is added.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the user's email address is confirmed.
        /// Admin can manually confirm user email addresses without requiring verification process.
        /// Maps to AspNetUsers.EmailConfirmed in the database.
        /// 
        /// When true: User can login and access the system
        /// When false: User may be blocked from login (depends on RequireConfirmedAccount setting)
        /// 
        /// In v1.0: This is set to true by default since email confirmation is not required.
        /// In v2.0: This will integrate with email verification workflow.
        /// </summary>
        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Collection of role names currently assigned to this user.
        /// Populated from AspNetUserRoles table when loading the edit page.
        /// Used to pre-select checkboxes in the role assignment UI.
        /// On form submission, this list contains the roles that should be assigned to the user.
        /// Example values: ["Admin", "Manager"] or ["User"]
        /// </summary>
        public List<string> SelectedRoles { get; set; } = new();

        /// <summary>
        /// Collection of all available role names in the system.
        /// Populated from AspNetRoles table when loading the edit page.
        /// Used to display all possible roles as checkboxes in the UI.
        /// Roles in SelectedRoles will have their checkboxes pre-checked.
        /// Example values: ["Admin", "Manager", "User", "CustomRole"]
        /// </summary>
        public List<string> AllRoles { get; set; } = new();
    }
}