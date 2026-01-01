using Microsoft.AspNetCore.Identity;

namespace UM_with_Role_Claim_Audit_log.Data
{
    /// <summary>
    /// Static class responsible for seeding initial data into the database.
    /// Seeds default roles (Admin, Manager, User) and creates a default admin account.
    /// This class is called automatically during application startup in Program.cs.
    /// 
    /// NOTE: This class does NOT seed claims. Claims are managed dynamically through the UI
    /// using the ClaimDefinitionsService, supporting the dynamic policy-based authorization system.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Initializes the database with default roles and admin user.
        /// This method is idempotent - it can be run multiple times safely without duplicating data.
        /// Automatically executed on application startup after database migrations.
        /// </summary>
        /// <param name="serviceProvider">Dependency injection service provider for accessing Identity services</param>
        /// <returns>Completed task when seeding is finished</returns>
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Retrieve Identity services from dependency injection container
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // ========== SEED DEFAULT ROLES ==========
            // Create three foundational roles for the authorization system:
            // - Admin: Full system access (bypasses all claim checks via ClaimBasedAuthorizationHandler)
            // - Manager: Mid-level access (assign specific claims via UI)
            // - User: Basic access (assign specific claims via UI)
            string[] roleNames = { "Admin", "Manager", "User" };

            foreach (var roleName in roleNames)
            {
                // Check if role already exists to prevent duplicates
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // Create new role in AspNetRoles table
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // ========== SEED DEFAULT ADMIN USER ==========
            // Create a default admin account for immediate system access after setup.
            // Buyers can login with these credentials and then create their own accounts.
            var adminEmail = "admin@demo.com";
            var adminPassword = "Admin@123"; // Meets default password requirements: uppercase, lowercase, digit, special char

            // Check if admin user already exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // Create new admin user account
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,      // Username set to email for simplicity
                    Email = adminEmail,          // Contact email
                    EmailConfirmed = true        // Skip email confirmation for demo account (v1.0 feature)
                };

                // Create user with password in AspNetUsers table
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                // If user creation succeeded, assign Admin role
                if (result.Succeeded)
                {
                    // Add user to Admin role (creates entry in AspNetUserRoles table)
                    // Admin role grants full system access via ClaimBasedAuthorizationHandler
                    await userManager.AddToRoleAsync(adminUser, "Admin");

                    // NOTE: No claims are seeded here!
                    // The Admin role automatically bypasses all claim checks.
                    // Other roles (Manager, User) receive claims through the UI at /RoleClaims/Manage
                    // Individual user claims are assigned at /Claims/Manage
                    // This supports the dynamic authorization system without hardcoded permissions.
                }
            }
        }
    }
}