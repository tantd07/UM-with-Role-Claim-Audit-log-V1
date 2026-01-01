using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UM_with_Role_Claim_Audit_log.Data;
using UM_with_Role_Claim_Audit_log.Services;
using UM_with_Role_Claim_Audit_log.Authorization; // ← ADD THIS
using Microsoft.AspNetCore.Authorization; // ← ADD THIS

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Disable email confirmation requirement for easier testing
    options.SignIn.RequireConfirmedAccount = false;

    // Password settings (optional - make it easier for development)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
    .AddRoles<IdentityRole>() // Add Role support
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ========== DYNAMIC POLICY-BASED AUTHORIZATION ==========
// Register the custom authorization handler
builder.Services.AddSingleton<IAuthorizationHandler, ClaimBasedAuthorizationHandler>();

// Register the dynamic policy provider
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicPolicyProvider>();

// You can still add custom policies here if needed for special cases
builder.Services.AddAuthorization(options =>
{
    // Example: Custom policy with multiple requirements
    // Most policies will be generated dynamically, so this section is minimal
});

builder.Services.AddScoped<AuditLogService>(); // Add Audit Log service

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed data - with error handling
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Ensure database is created and migrations are applied
        await context.Database.MigrateAsync();

        // Seed data
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Add security headers (AFTER app.UseHttpsRedirection())
app.Use(async (context, next) =>
{
    // ✅ Basic Security Headers
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

    // ✅ CSP - Allow Bootstrap Icons CDN
    var csp = "default-src 'self'; " +
              "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
              "font-src 'self' https://cdn.jsdelivr.net data:; " +
              "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
              "img-src 'self' data: https:; " +
              "connect-src 'self';";

    context.Response.Headers["Content-Security-Policy"] = csp;

    // ✅ Remove revealing headers
    context.Response.Headers.Remove("Server");
    context.Response.Headers.Remove("X-Powered-By");

    await next();
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();