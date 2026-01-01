using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UM_with_Role_Claim_Audit_log.Models;

namespace UM_with_Role_Claim_Audit_log.Data
{
    /// <summary>
    /// Application Database Context - Main EF Core DbContext for the User Management System
    /// Extends IdentityDbContext to provide ASP.NET Core Identity functionality with custom audit logging
    /// </summary>
    /// <remarks>
    /// This context manages:
    /// - ASP.NET Core Identity tables (Users, Roles, UserRoles, UserClaims, RoleClaims, etc.)
    /// - Custom AuditLog table for tracking all user/role/claim changes
    /// - Database connection and entity configurations
    /// </remarks>
    public class ApplicationDbContext : IdentityDbContext
    {
        /// <summary>
        /// Constructor: Initializes the ApplicationDbContext with configuration options
        /// </summary>
        /// <param name="options">Database context configuration options (connection string, provider, etc.)</param>
        /// <remarks>
        /// Options are typically configured in Program.cs/Startup.cs using:
        /// services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        /// </remarks>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            // Pass options to base IdentityDbContext constructor
            // This sets up all Identity-related tables (AspNetUsers, AspNetRoles, etc.)
        }

        /// <summary>
        /// AuditLogs DbSet - Table for storing audit trail of all system changes
        /// </summary>
        /// <remarks>
        /// Tracks:
        /// - User creation, updates, deletion, and status changes
        /// - Role assignments and removals
        /// - Claim assignments (both user claims and role claims)
        /// - Who made the change (Actor)
        /// - When the change occurred (Timestamp)
        /// - What was changed (Action description)
        /// 
        /// This provides full accountability and compliance tracking for security audits
        /// </remarks>
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Note: Identity tables are inherited from IdentityDbContext:
        // - Users (AspNetUsers)
        // - Roles (AspNetRoles)
        // - UserRoles (AspNetUserRoles) - Many-to-many relationship
        // - UserClaims (AspNetUserClaims) - Custom permissions per user
        // - RoleClaims (AspNetRoleClaims) - Custom permissions per role
        // - UserLogins (AspNetUserLogins) - External authentication providers
        // - UserTokens (AspNetUserTokens) - Authentication tokens
        // - RoleClaims (AspNetRoleClaims) - Claims assigned to roles
    }
}