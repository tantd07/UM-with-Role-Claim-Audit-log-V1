namespace UM_with_Role_Claim_Audit_log.Models.ViewModels
{
    /// <summary>
    /// View model for displaying audit log entries in the audit logs list view.
    /// Used in AuditLogs/Index to present audit trail data in a user-friendly format.
    /// 
    /// Purpose:
    /// Enhances the AuditLog entity with human-readable user identification.
    /// Resolves UserId (GUID) to UserEmail for better readability in the UI.
    /// 
    /// Key Difference from AuditLog Entity:
    /// - AuditLog: Database entity with UserId as GUID string
    /// - AuditLogViewModel: Display model with UserEmail resolved from UserId
    /// 
    /// Transformation Flow:
    /// 1. AuditLog loaded from database (has UserId)
    /// 2. Controller resolves UserId to email via UserManager.FindByIdAsync()
    /// 3. AuditLogViewModel created with user email for display
    /// 4. View displays "admin@company.com" instead of "a1b2c3d4-e5f6-..."
    /// 
    /// Usage Context:
    /// - AuditLogs/Index: Paginated list of audit log entries
    /// - Each row shows: User Email | Action | Timestamp | Details
    /// - Sorted by Timestamp descending (most recent first)
    /// 
    /// Why Separate ViewModel?
    /// - Separation of concerns (database entity vs display model)
    /// - UserEmail not stored in database (computed at display time)
    /// - Handles deleted users gracefully (email = "Unknown")
    /// - Clean architecture pattern (entity → ViewModel → view)
    /// 
    /// Related Files:
    /// - Models/AuditLog.cs: Database entity (source data)
    /// - Controllers/AuditLogsController.cs: Transforms entity to ViewModel
    /// - Views/AuditLogs/Index.cshtml: Displays the ViewModel
    /// - Services/AuditLogService.cs: Creates AuditLog entities
    /// </summary>
    public class AuditLogViewModel
    {
        /// <summary>
        /// The unique identifier of the audit log entry.
        /// Maps directly from AuditLog.Id (primary key in database).
        /// 
        /// Purpose:
        /// - Identifies individual log entries uniquely
        /// - Used for pagination and record identification
        /// - Potential future use: Drill-down to detailed log view
        /// 
        /// Type: int (auto-increment in database)
        /// 
        /// Display:
        /// Typically NOT displayed in the UI (used internally).
        /// Users see timestamp and action, not database ID.
        /// Could be shown in a "Log Entry #12345" format if needed.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The unique identifier of the user who performed the action.
        /// Maps directly from AuditLog.UserId (GUID string).
        /// 
        /// Purpose:
        /// Retained from the entity for reference and filtering purposes.
        /// Not typically displayed in UI (UserEmail is shown instead).
        /// 
        /// Format:
        /// GUID string (e.g., "a1b2c3d4-e5f6-7890-abcd-1234567890ab")
        /// 
        /// Special Values:
        /// - Normal user: GUID from AspNetUsers.Id
        /// - System actions: "System"
        /// - Anonymous: "Anonymous"
        /// - Unknown: "Unknown"
        /// 
        /// Use Cases:
        /// - Backend filtering: Get all logs by specific UserId
        /// - User activity reports: Filter by UserId parameter
        /// - Data export: Include UserId for technical reference
        /// - Troubleshooting: Link log entries to user accounts
        /// 
        /// Display:
        /// Not shown in standard UI (replaced by UserEmail for readability).
        /// May be included in:
        /// - CSV exports for technical analysis
        /// - Advanced admin tools
        /// - API responses
        /// - Debug/troubleshooting views
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the user who performed the action.
        /// Resolved from UserId via UserManager.FindByIdAsync() in the controller.
        /// 
        /// Purpose:
        /// Provides human-readable user identification in the audit log UI.
        /// Shows "john.doe@company.com" instead of cryptic GUID.
        /// 
        /// Resolution Process (in AuditLogsController):
        /// 1. Load AuditLog from database (has UserId)
        /// 2. var user = await _userManager.FindByIdAsync(log.UserId);
        /// 3. var email = user?.Email ?? "Unknown";
        /// 4. Set UserEmail = email in ViewModel
        /// 
        /// Special Cases:
        /// 
        /// User Deleted:
        /// If user no longer exists in AspNetUsers table:
        /// - UserManager.FindByIdAsync() returns null
        /// - UserEmail set to "Unknown"
        /// - Audit log preserved (historical data intact)
        /// 
        /// System Actions:
        /// If UserId = "System":
        /// - FindByIdAsync() returns null
        /// - UserEmail displayed as "System" or "Unknown"
        /// - Consider special handling in controller for system accounts
        /// 
        /// Anonymous Actions:
        /// If UserId = "Anonymous":
        /// - Display as "Anonymous User"
        /// - Indicates action by unauthenticated user
        /// 
        /// Display Format:
        /// - Standard: "john.doe@company.com" (clickable link to user details)
        /// - Deleted: "Unknown (User ID: abc123...)" with tooltip
        /// - System: "System Administrator" with icon
        /// - Anonymous: "Anonymous" with different styling
        /// 
        /// UI Presentation:
        /// Typically shown in the first or second column of audit log table:
        /// | User Email           | Action        | Time        | Details          |
        /// | john.doe@company.com | User Created  | 2 hours ago | Created user:... |
        /// 
        /// Clickable Link:
        /// Can link to user details page: /Users/Details/{userId}
        /// Enables quick navigation: "Who is this user? Click to see profile"
        /// 
        /// Filtering:
        /// Enable UI to filter logs by email:
        /// - Search box: "Show logs for john.doe@company.com"
        /// - Dropdown: Select user from list
        /// - Behind the scenes: Filter by UserId, not email (email might change)
        /// 
        /// Performance Note:
        /// Email resolution happens in controller, not in database query.
        /// For large result sets (100+ logs), this requires 100+ user lookups.
        /// Consider optimization:
        /// - Batch load users: var userIds = logs.Select(l => l.UserId).Distinct();
        /// - Cache user emails in memory during page rendering
        /// - Join query: Include user data in initial query (advanced)
        /// 
        /// Privacy Consideration:
        /// User email is sensitive PII (Personally Identifiable Information).
        /// Access to audit logs should be restricted (CanViewAuditLogs claim).
        /// Consider masking or redacting emails based on viewer's permissions.
        /// </summary>
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>
        /// Brief description of the action that was performed.
        /// Maps directly from AuditLog.Action.
        /// 
        /// Purpose:
        /// Categorizes the log entry for filtering and quick identification.
        /// 
        /// Example Values:
        /// - "User Created" - New user account created
        /// - "User Updated" - User profile modified
        /// - "User Deleted" - User account removed
        /// - "Password Reset" - Admin reset user password
        /// - "Role Created" - New role created
        /// - "User Claims Updated" - User permissions modified
        /// - "Role Claims Updated" - Role permissions modified
        /// 
        /// Display in UI:
        /// Shown as a prominent column in the audit log table.
        /// Often styled with badges or icons:
        /// - "User Created" → Green badge with + icon
        /// - "User Deleted" → Red badge with trash icon
        /// - "Password Reset" → Yellow badge with key icon
        /// 
        /// Filtering:
        /// Enable dropdown or search to filter by action type:
        /// - "Show only 'Password Reset' actions"
        /// - "Show all user management actions"
        /// - "Show all claim updates"
        /// 
        /// Sorting:
        /// Can be sorted alphabetically to group similar actions together.
        /// Default sort is by Timestamp (most recent first).
        /// 
        /// Consistency:
        /// Action names should follow standardized naming from AuditLog.cs comments.
        /// Ensures consistent categorization across the system.
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// The date and time when the action was performed.
        /// Maps directly from AuditLog.Timestamp (stored as UTC in database).
        /// 
        /// Purpose:
        /// Provides chronological context for the audit log entry.
        /// Primary sorting field (most recent first in UI).
        /// 
        /// Data Type: DateTime
        /// Stored as UTC in database, may be converted for display.
        /// 
        /// Display Formats:
        /// 
        /// 1. Relative Time (Recommended for recent logs):
        ///    - "2 minutes ago"
        ///    - "1 hour ago"
        ///    - "Yesterday at 3:45 PM"
        ///    - "Last week"
        ///    
        ///    Implementation:
        ///    Use JavaScript library (e.g., moment.js) or C# helper method
        ///    @TimeAgo(Model.Timestamp) // Custom helper
        ///    
        /// 2. Absolute Time (For older logs or exports):
        ///    - "2025-12-29 14:30:45" (ISO format)
        ///    - "Dec 29, 2025 at 2:30 PM" (friendly format)
        ///    - "12/29/2025 14:30" (short format)
        ///    
        ///    Implementation:
        ///    @Model.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
        ///    @Model.Timestamp.ToString("MMM dd, yyyy 'at' h:mm tt")
        ///    
        /// 3. User's Local Time (Convert from UTC):
        ///    - @Model.Timestamp.ToLocalTime()
        ///    - Or use JavaScript to convert to browser timezone
        ///    - Display timezone indicator: "2:30 PM PST"
        /// 
        /// Sorting:
        /// Default sort: ORDER BY Timestamp DESC (most recent first)
        /// This is the most common view for audit logs.
        /// Users want to see latest activities first.
        /// 
        /// Filtering by Date Range:
        /// Enable UI controls to filter by date:
        /// - "Last 24 hours"
        /// - "Last 7 days"
        /// - "Last 30 days"
        /// - "Custom range: From [date] To [date]"
        /// 
        /// Example UI:
        /// | User Email           | Action       | Timestamp      | Details          |
        /// | admin@company.com    | User Created | 2 hours ago    | Created user:... |
        /// | manager@company.com  | Role Updated | Yesterday      | Updated role:... |
        /// | admin@company.com    | Password Reset | 3 days ago   | Reset password...|
        /// 
        /// Tooltip Enhancement:
        /// Show relative time in column, full timestamp on hover:
        /// Display: "2 hours ago"
        /// Tooltip: "December 29, 2025 at 2:30:45 PM UTC"
        /// 
        /// CSV Export:
        /// Use full ISO 8601 format for exports:
        /// "2025-12-29T14:30:45Z" (standardized format)
        /// 
        /// Performance:
        /// Timestamp is indexed in database for fast sorting and filtering.
        /// Queries with date ranges execute efficiently.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Additional context and information about the action.
        /// Maps directly from AuditLog.Details.
        /// 
        /// Purpose:
        /// Provides detailed description of what happened.
        /// Contains variable data (affected user, changed values, etc.).
        /// 
        /// Example Content:
        /// - "Created user: john.doe@company.com"
        /// - "Updated user: jane@company.com. Changed email from old@email.com to new@email.com"
        /// - "Deleted role: TemporaryRole (had 0 users)"
        /// - "Updated claims for role: Manager. Claims: CanEditUsers, CanViewRoles, CanManageClaims"
        /// - "Admin reset password for user: support@company.com"
        /// 
        /// Display in UI:
        /// 
        /// 1. List View (Truncated):
        ///    Show first 50-100 characters with "..." if longer:
        ///    "Created user: john.doe@company.com"
        ///    "Updated claims for role: Manager. Claims: CanEditUsers, CanView..."
        ///    
        ///    Implementation:
        ///    @(Model.Details.Length > 100 
        ///        ? Model.Details.Substring(0, 100) + "..." 
        ///        : Model.Details)
        ///    
        /// 2. Expandable Details:
        ///    Click "View Details" to expand full content:
        ///    - Modal popup with full text
        ///    - Expandable row in table
        ///    - Tooltip on hover
        ///    
        /// 3. Details Page (Future Enhancement):
        ///    Click log entry to see full details:
        ///    /AuditLogs/Details/{id}
        ///    Shows complete Details text with formatting
        /// 
        /// Formatting:
        /// Consider special formatting for structured details:
        /// - Highlight emails: john.doe@company.com (clickable)
        /// - Highlight role/claim names in bold
        /// - Use line breaks for multi-item lists
        /// - Syntax highlighting for JSON (if using JSON format)
        /// 
        /// Searchability:
        /// Enable full-text search in Details:
        /// - Search box: "Find logs containing: john.doe@company.com"
        /// - Highlights matching text in results
        /// - Supports partial matches
        /// 
        /// Example Search Query (Backend):
        /// var searchTerm = "john.doe@company.com";
        /// var logs = _context.AuditLogs
        ///     .Where(l => l.Details.Contains(searchTerm))
        ///     .OrderByDescending(l => l.Timestamp)
        ///     .ToList();
        /// 
        /// Copy to Clipboard:
        /// Add "Copy" button to copy details text:
        /// - Useful for sharing with support team
        /// - Include in incident reports
        /// - Paste into documentation
        /// 
        /// CSV Export:
        /// Include full Details text in exported CSV:
        /// - Properly escape commas and quotes
        /// - Use Excel-compatible format
        /// - Consider separate column for each detail component
        /// 
        /// Performance Note:
        /// Details can be lengthy (up to 2000+ characters).
        /// List view loads all Details even if truncated (potential optimization).
        /// Consider lazy loading or pagination for very large result sets.
        /// 
        /// Privacy Consideration:
        /// Details may contain sensitive information (user emails, role names).
        /// Ensure proper access control (CanViewAuditLogs claim required).
        /// Avoid logging passwords or other secrets in Details field.
        /// </summary>
        public string Details { get; set; } = string.Empty;
    }
}