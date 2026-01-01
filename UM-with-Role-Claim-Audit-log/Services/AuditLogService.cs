using UM_with_Role_Claim_Audit_log.Data;
using UM_with_Role_Claim_Audit_log.Models;

namespace UM_with_Role_Claim_Audit_log.Services
{
    /// <summary>
    /// Centralized service for creating audit log entries throughout the application.
    /// Provides a consistent interface for logging all user actions and system events.
    /// 
    /// Purpose:
    /// - Single point of responsibility for audit logging (Single Responsibility Principle)
    /// - Ensures consistent audit log formatting and storage
    /// - Simplifies logging across all controllers and services
    /// - Enables easy future enhancements (e.g., adding IP address, user agent, etc.)
    /// 
    /// Key Features:
    /// - Automatic UTC timestamp recording
    /// - Asynchronous database operations for performance
    /// - Optional details parameter for flexibility
    /// - Used throughout the application via dependency injection
    /// 
    /// Dependency Injection:
    /// Registered in Program.cs as scoped service:
    /// builder.Services.AddScoped<AuditLogService>();
    /// 
    /// Controllers inject this service via constructor:
    /// private readonly AuditLogService _auditLogService;
    /// public UsersController(AuditLogService auditLogService) { ... }
    /// 
    /// Usage Pattern Across Application:
    /// 
    /// User Management:
    /// await _auditLogService.LogAsync(
    ///     currentUserId, 
    ///     "User Created", 
    ///     $"Created user: {newUser.Email}"
    /// );
    /// 
    /// Role Management:
    /// await _auditLogService.LogAsync(
    ///     currentUserId,
    ///     "Role Claims Updated",
    ///     $"Updated claims for role: {role.Name}. Claims: {claimsList}"
    /// );
    /// 
    /// Password Reset:
    /// await _auditLogService.LogAsync(
    ///     currentUserId,
    ///     "Password Reset",
    ///     $"Admin reset password for user: {user.Email}"
    /// );
    /// 
    /// Benefits of Centralized Logging:
    /// - Consistency: All logs follow the same structure
    /// - Maintainability: Change logging logic in one place
    /// - Testability: Easy to mock for unit tests
    /// - Extensibility: Add features (filtering, batching) without changing callers
    /// - Performance: Can add batching, queuing, or async processing
    /// 
    /// Future Enhancements (Easy to Add):
    /// - IP Address tracking: Add IPAddress property to AuditLog
    /// - User Agent: Track browser/device information
    /// - Request Path: Log which endpoint was called
    /// - Session ID: Track user sessions
    /// - Severity Levels: Info, Warning, Error
    /// - Async Queue: Batch writes for high-traffic scenarios
    /// - External Logging: Send to Serilog, Application Insights, etc.
    /// 
    /// Security Considerations:
    /// - Never log passwords or sensitive credentials
    /// - Be cautious with PII (Personally Identifiable Information)
    /// - Ensure audit logs themselves are protected (CanViewAuditLogs claim)
    /// - Consider encryption at rest for compliance
    /// 
    /// Performance Note:
    /// Uses async/await for non-blocking database writes.
    /// For extremely high-traffic scenarios, consider:
    /// - Background queue processing (IBackgroundTaskQueue)
    /// - Batching multiple log entries
    /// - Using ILogger with custom providers
    /// 
    /// Related Files:
    /// - Models/AuditLog.cs: Entity model for audit log entries
    /// - Controllers/*: All controllers use this service to log actions
    /// - Data/ApplicationDbContext.cs: DbContext includes AuditLogs DbSet
    /// - Views/AuditLogs/Index.cshtml: Displays logged entries
    /// </summary>
    public class AuditLogService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the AuditLogService with database context.
        /// 
        /// Dependency Injection:
        /// This constructor is called by ASP.NET Core's DI container.
        /// ApplicationDbContext is automatically injected as a scoped service.
        /// 
        /// Service Lifetime:
        /// AuditLogService is registered as Scoped (same as DbContext):
        /// - One instance per HTTP request
        /// - Shared across all components in the same request
        /// - Disposed at end of request
        /// 
        /// Why Scoped?
        /// - Matches DbContext lifetime (best practice)
        /// - Prevents threading issues with DbContext
        /// - Efficient memory usage (not singleton, not transient)
        /// </summary>
        /// <param name="context">Database context for accessing AuditLogs table</param>
        public AuditLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new audit log entry and saves it to the database.
        /// This is the primary method called throughout the application to log user actions.
        /// 
        /// Purpose:
        /// Records who did what, when, and with what additional context.
        /// Provides complete audit trail for security, compliance, and troubleshooting.
        /// 
        /// Process Flow:
        /// 1. Create new AuditLog entity with provided parameters
        /// 2. Set Timestamp to current UTC time (automatic, not passed by caller)
        /// 3. Add to database context (in-memory, not yet saved)
        /// 4. Save changes asynchronously to database (commit transaction)
        /// 
        /// Asynchronous Operation:
        /// Uses async/await pattern for non-blocking database I/O.
        /// Allows other requests to be processed while waiting for database write.
        /// Essential for scalability in web applications.
        /// 
        /// Usage Examples Across Application:
        /// 
        /// 1. User Management (UsersController):
        /// 
        ///    User Created:
        ///    await _auditLogService.LogAsync(
        ///        _userManager.GetUserId(User),
        ///        "User Created",
        ///        $"Created user: {model.Email}"
        ///    );
        ///    
        ///    User Updated:
        ///    await _auditLogService.LogAsync(
        ///        currentUserId,
        ///        "User Updated",
        ///        $"Updated user: {model.Email}"
        ///    );
        ///    
        ///    User Deleted:
        ///    await _auditLogService.LogAsync(
        ///        currentUserId,
        ///        "User Deleted",
        ///        $"Deleted user: {user.Email}"
        ///    );
        ///    
        ///    Password Reset:
        ///    await _auditLogService.LogAsync(
        ///        currentUserId,
        ///        "Password Reset",
        ///        $"Admin reset password for user: {user.Email}"
        ///    );
        /// 
        /// 2. Role Management (RolesController):
        /// 
        ///    Role Created:
        ///    await _auditLogService.LogAsync(
        ///        currentUserId,
        ///        "Role Created",
        ///        $"Created role: {model.Name}"
        ///    );
        ///    
        ///    Role Deleted:
        ///    await _auditLogService.LogAsync(
        ///        currentUserId,
        ///        "Role Deleted",
        ///        $"Deleted role: {role.Name}"
        ///    );
        /// 
        /// 3. Claims Management (ClaimsController, RoleClaimsController):
        /// 
        ///    User Claims Updated:
        ///    await _auditLogService.LogAsync(
        ///        currentUserId,
        ///        "User Claims Updated",
        ///        $"Updated claims for user: {model.UserEmail}. Direct claims: {claimsList}"
        ///    );
        ///    
        ///    Role Claims Updated:
        ///    await _auditLogService.LogAsync(
        ///        currentUserId,
        ///        "Role Claims Updated",
        ///        $"Updated claims for role: {role.Name}. Claims: {claimsList}"
        ///    );
        /// 
        /// Parameter Guidelines:
        /// 
        /// userId:
        /// - Use _userManager.GetUserId(User) to get current user's ID
        /// - For system actions, use "System"
        /// - For anonymous actions, use "Anonymous"
        /// - Never pass null (use "Unknown" if unavailable)
        /// 
        /// action:
        /// - Use standardized action names (see AuditLog.cs for list)
        /// - Use past tense: "User Created" not "Create User"
        /// - Keep concise but descriptive
        /// - Examples: "User Created", "Role Updated", "Password Reset"
        /// 
        /// details:
        /// - Optional parameter (defaults to empty string)
        /// - Include affected entity and key changes
        /// - Format: "{Action} for {Entity}: {Identifier}. {AdditionalContext}"
        /// - Example: "Updated user: john@company.com. Changed email from old@email.com"
        /// - Avoid logging sensitive data (passwords, API keys)
        /// 
        /// Error Handling:
        /// Currently no explicit error handling (database exceptions propagate to caller).
        /// Caller should wrap in try-catch if needed:
        /// 
        /// try
        /// {
        ///     await _auditLogService.LogAsync(userId, action, details);
        /// }
        /// catch (Exception ex)
        /// {
        ///     // Log error but don't fail the main operation
        ///     _logger.LogError(ex, "Failed to create audit log");
        /// }
        /// 
        /// Alternative: Add try-catch inside this method (swallow exceptions).
        /// Trade-off: Silent failures vs. bubbling errors to caller.
        /// 
        /// Future Enhancements:
        /// 
        /// 1. Add IP Address:
        ///    public async Task LogAsync(string userId, string action, string details, string ipAddress)
        ///    {
        ///        var auditLog = new AuditLog { ..., IPAddress = ipAddress };
        ///    }
        ///    
        /// 2. Add User Agent:
        ///    public async Task LogAsync(..., string userAgent)
        ///    
        /// 3. Add Severity:
        ///    public async Task LogAsync(..., LogLevel level = LogLevel.Information)
        ///    
        /// 4. Async Queue (High Traffic):
        ///    Add to background queue instead of immediate save
        ///    Process batch writes in background task
        ///    
        /// 5. External Logging Integration:
        ///    Also log to Serilog, Application Insights, etc.
        ///    Centralized logging across multiple systems
        /// 
        /// Performance Considerations:
        /// - Async operation: Non-blocking, good for scalability
        /// - Database write per call: Acceptable for typical traffic
        /// - High traffic: Consider batching or background queue
        /// - Indexing: Ensure AuditLogs table has indexes on UserId, Timestamp, Action
        /// 
        /// Testing:
        /// Easy to mock for unit tests:
        /// 
        /// var mockService = new Mock<AuditLogService>();
        /// mockService.Setup(s => s.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        ///            .Returns(Task.CompletedTask);
        /// 
        /// Compliance & Security:
        /// - Audit logs must be immutable (no edit/delete in UI)
        /// - Access restricted to users with CanViewAuditLogs claim
        /// - Retain logs per regulatory requirements (e.g., 90 days, 1 year)
        /// - Consider archival strategy for old logs
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user performing the action.
        /// Typically obtained via: _userManager.GetUserId(User)
        /// For system actions, use: "System"
        /// For anonymous, use: "Anonymous"
        /// Must not be null or empty (use "Unknown" if unavailable).
        /// </param>
        /// <param name="action">
        /// Brief description of the action performed.
        /// Use standardized action names for consistency:
        /// - "User Created", "User Updated", "User Deleted"
        /// - "Role Created", "Role Updated", "Role Deleted"
        /// - "User Claims Updated", "Role Claims Updated"
        /// - "Password Reset", "Account Locked", "Account Unlocked"
        /// Use past tense. Keep concise but descriptive.
        /// </param>
        /// <param name="details">
        /// Optional additional context about the action.
        /// Provides variable data and specific information.
        /// Format: "Created user: john@company.com" or 
        ///         "Updated claims for role: Manager. Claims: CanEditUsers, CanViewRoles"
        /// Avoid logging sensitive data (passwords, API keys, SSNs).
        /// Defaults to empty string if not provided.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// Completes when the audit log entry has been saved to the database.
        /// No return value (void task).
        /// </returns>
        public async Task LogAsync(string userId, string action, string details = "")
        {
            // Create new audit log entity with provided data
            var auditLog = new AuditLog
            {
                UserId = userId,              // Who performed the action
                Action = action,              // What action was performed
                Timestamp = DateTime.UtcNow,  // When it happened (UTC for consistency)
                Details = details             // Additional context (optional)
            };

            // Add to database context (in-memory, not yet persisted)
            _context.AuditLogs.Add(auditLog);

            // Save changes to database (commit transaction)
            // This is an async operation - doesn't block the thread
            await _context.SaveChangesAsync();
        }
    }
}