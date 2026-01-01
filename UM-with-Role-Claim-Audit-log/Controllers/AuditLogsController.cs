using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UM_with_Role_Claim_Audit_log.Data;
using UM_with_Role_Claim_Audit_log.Models.ViewModels;

namespace UM_with_Role_Claim_Audit_log.Controllers
{
    /// <summary>
    /// Controller responsible for displaying audit logs and activity tracking.
    /// Provides read-only access to system audit trail for authorized users.
    /// All actions require CanViewAuditLogsPolicy authorization.
    /// </summary>
    [Authorize(Policy = "CanViewAuditLogsPolicy")] // Admin, Manager, or users with CanViewAuditLogs claim
    public class AuditLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the AuditLogsController with required dependencies.
        /// </summary>
        /// <param name="context">Database context for accessing audit log data</param>
        /// <param name="userManager">Identity user manager for resolving user information</param>
        public AuditLogsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Displays a paginated list of audit logs showing all system activities.
        /// Logs are ordered by timestamp (most recent first) and include user email resolution.
        /// Supports pagination to handle large audit log datasets efficiently.
        /// </summary>
        /// <param name="page">Current page number (default: 1)</param>
        /// <param name="pageSize">Number of logs per page (default: 20)</param>
        /// <returns>View with paginated audit log list and pagination metadata</returns>
        // GET: AuditLogs
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            // Get total count of audit logs for pagination calculation
            var totalLogs = await _context.AuditLogs.CountAsync();

            // Retrieve paginated audit logs ordered by most recent first
            // Skip: Skips records from previous pages
            // Take: Limits the number of records to current page size
            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.Timestamp) // Most recent logs first
                .Skip((page - 1) * pageSize)         // Skip previous pages
                .Take(pageSize)                      // Get current page records
                .ToListAsync();

            // Build view models with user email information
            var viewModels = new List<AuditLogViewModel>();

            // Resolve user email for each audit log entry
            // This provides human-readable user identification in the UI
            foreach (var log in logs)
            {
                // Look up user by ID to get email address
                var user = await _userManager.FindByIdAsync(log.UserId);

                // Create view model with enriched user data
                viewModels.Add(new AuditLogViewModel
                {
                    Id = log.Id,
                    UserId = log.UserId,
                    UserEmail = user?.Email ?? "Unknown", // Fallback to "Unknown" if user not found
                    Action = log.Action,                  // What action was performed (e.g., "User Created")
                    Timestamp = log.Timestamp,            // When the action occurred
                    Details = log.Details                 // Additional context about the action
                });
            }

            // Pass pagination metadata to the view for rendering page navigation
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize); // Calculate total pages

            return View(viewModels);
        }
    }
}