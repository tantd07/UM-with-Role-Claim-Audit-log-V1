# 🚀 User Management System with Dynamic Role-Based & Claims-Based Authorization

**ASP.NET Core 9 MVC | Production-Ready | Dynamic Authorization System**

A complete, enterprise-grade user management system with **dynamic policy-based authorization**. 
No hardcoded policies - permissions are evaluated at runtime based on roles and claims!

> **⚠️ Current Version:** This is Version 1.0 - A solid foundation for production use. Email confirmation and Two-Factor Authentication (2FA) will be added in future updates, along with enhanced UI/UX improvements.

---

## 🔗 Live Demo & Video Walkthrough

- 🌐 **Live Demo (V1.0)**: https://your-demo-v1-url
- 🎥 **Video Walkthrough**: https://your-video-demo-url

> You can explore the full system, test roles, claims, and audit logs before purchasing.

---

## 🎯 What Makes This Different?

### Revolutionary Authorization System
Unlike traditional ASP.NET Core authorization that requires hardcoded policies in `Program.cs`, this system uses **Dynamic Policy Provider** that:
- ✅ **Automatically generates policies at runtime** based on claim requirements
- ✅ **No need to register policies manually** in `Program.cs`
- ✅ **Evaluates user roles AND claims together** for flexible permission control
- ✅ **Supports both role inheritance and direct user claims**
- ✅ **Easy to extend** - just add new claim types without touching startup code

### How It Works:
// Traditional Way (NOT used here): // You would need to register every policy manually: builder.Services.AddAuthorization(options => 
{ options.AddPolicy("CanEditUsersPolicy", policy => policy.RequireClaim("CanEditUsers", "true")); 
options.AddPolicy("CanDeleteUsersPolicy", policy => policy.RequireClaim("CanDeleteUsers", "true")); // ... dozens more policies ... });

// ✨ Our Dynamic Way (What we built): // Just use any policy name in your controller: [Authorize(Policy = "CanEditUsersPolicy")] 
// Works automatically! [Authorize(Policy = "CanDeleteUsersPolicy")] // No registration needed! [Authorize(Policy = "YourCustomClaimPolicy")] 
// Just add it and it works!

// The DynamicPolicyProvider handles everything behind the scenes!

---

## 👤 Who Should Buy This?

### Perfect For:
- **ASP.NET Core Developers** building internal business applications
- **Freelancers** who need a ready-to-integrate user management system
- **Startups & SaaS** requiring sophisticated permission controls
- **Enterprise Applications** with complex authorization requirements
- **Developers Learning** modern ASP.NET Core security patterns

### Use Cases:
- 🏢 Internal company portals (HR, Admin, CRM, ERP)
- 💼 Multi-tenant SaaS applications
- 🛒 E-commerce admin panels
- 📊 Business intelligence dashboards
- 🎓 Educational platforms with role-based access
- 🏥 Healthcare management systems

---

## ❌ What This Is NOT

- ❌ **Not JWT/API-based** - This is session-based MVC authentication (perfect for web apps)
- ❌ **Not a microservices architecture** - Clean monolithic MVC structure
- ❌ **Not a NuGet package** - Full source code you can customize
- ❌ **Not production UI/UX** - Functional Bootstrap 5 interface (UI enhancements coming in v2.0)

> **Note:** The UI is clean and functional but prioritizes functionality over design. Perfect for internal systems or as a starting point for your custom design.

---

## ❌ This Project Is NOT For

- Beginners looking for a simple CRUD tutorial
- Projects that only need basic role = string authorization
- Teams expecting a polished UI without customization

---

## ✨ Core Features

### 🔐 Authentication & Authorization
- ✅ **ASP.NET Core Identity** - Microsoft's battle-tested authentication
- ✅ **Session-based authentication** - No complex JWT setup
- ✅ **Dynamic policy-based authorization** - Runtime permission evaluation
- ✅ **Role-based access control (RBAC)** - Traditional role assignments
- ✅ **Claims-based access control (CBAC)** - Granular permissions
- ✅ **Hybrid authorization** - Roles + Claims working together

> **⚠️ Email Confirmation & 2FA:** Not included in v1.0. These advanced features will be added in v2.0 along with UI/UX improvements.

### 👥 User Management
- ✅ **Full CRUD operations** - Create, Read, Update, Delete users
- ✅ **User activation/deactivation** - Soft account locks
- ✅ **Password reset** - Admin can reset any user's password
- ✅ **View user details** - See assigned roles, claims, and activity
- ✅ **User registration** - New users can sign up (no email confirmation required in v1.0)

### 🛡️ Role Management
- ✅ **Create custom roles** - Beyond Admin, Manager, User
- ✅ **Assign/remove roles** - Flexible role assignments per user
- ✅ **Role-based claims** - Permissions inherited by all role members
- ✅ **View role details** - See users and claims for each role
- ✅ **Delete roles** - Clean removal from all users

### 🔑 Claims Management
- ✅ **User-specific claims** - Direct permission assignments
- ✅ **Role-based claims** - Permissions for entire role
- ✅ **Claim inheritance** - Users get claims from their roles
- ✅ **Visual claim indicators** - See inherited vs. direct claims
- ✅ **Read-only claim viewing** - For users without management permissions

### 📝 Audit Logging
- ✅ **Complete activity tracking** - Who did what, when
- ✅ **User actions logged** - Create, Update, Delete, Password Reset
- ✅ **Role changes logged** - Role assignments/removals
- ✅ **Claim modifications logged** - Permission changes tracked
- ✅ **Pagination support** - Easy navigation through logs

---

## 🔒 Security-First Design

This project is built with **security as a top priority**, implementing industry-standard protections:

### Built-in Security Features
- ✅ **OWASP Top 10 2021 Compliance** - Protected against common web vulnerabilities
- ✅ **HTTP Security Headers** - CSP, X-Frame-Options, X-Content-Type-Options configured
- ✅ **Password Security** - PBKDF2-HMAC-SHA256 hashing with per-password salt
- ✅ **SQL Injection Protection** - EF Core parameterized queries
- ✅ **XSS Prevention** - Razor automatic HTML encoding
- ✅ **CSRF Protection** - Anti-forgery tokens on all forms
- ✅ **Secure Sessions** - HttpOnly + Secure cookies
- ✅ **HTTPS Enforcement** - Automatic redirection configured
- ✅ **Audit Trail** - Complete activity logging for compliance

### Security Code Example
// Program.cs - Security headers configured out-of-the-box app.Use(async (context, next) => 
    { 
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1;
        mode=block";
        context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; ...";
        context.Response.Headers.Remove("Server"); // Hide server info await next(); 
    });

**Production-Ready Security:** Deploy with confidence - all critical security measures are pre-configured.

> **📖 For detailed security documentation, see:** [FEATURES.md - Security Section](#-security-features)

---

## 🔒 Current Authentication Status (v1.0)

### ✅ What's Enabled:
- **Immediate Login** - No email confirmation required (`RequireConfirmedAccount = false`)
- **Session-based authentication** - Secure cookie-based sessions
- **Password security** - Hashed passwords using Identity defaults
- **"Remember Me"** functionality - Persistent login sessions

### ⏳ Coming in v2.0:
- **Email Confirmation** - Verify user email addresses before activation
- **Two-Factor Authentication (2FA)** - SMS or authenticator app support
- **Email Recovery** - Password reset via email link
- **External Logins** - Google, Microsoft, Facebook integration
- **Enhanced UI/UX** - Modern, polished interface with better animations

> **Why Not Now?** Version 1.0 focuses on rock-solid authorization logic and core functionality. Email/2FA features require additional setup (SMTP configuration, external services) which adds complexity. We're keeping v1.0 simple and production-ready for internal systems, while v2.0 will target customer-facing applications.

---

## 📋 Requirements

### Development Environment:
- **Visual Studio 2022 or later** (or VS Code with C# Dev Kit)
- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server LocalDB** (included with Visual Studio) or SQL Server Express/Full

### Knowledge Requirements:
- Basic understanding of ASP.NET Core MVC
- Familiarity with Entity Framework Core
- Basic SQL Server knowledge

---

## ⚡ Quick Start Guide (15 Minutes)

### Step 1: Download & Extract
1. Download the ZIP file from Gumroad
2. Extract to your preferred location (e.g., `C:\Projects\UserManagement`)
3. Unblock the files if prompted by Windows

### Step 2: Open in Visual Studio
1. Open `UM-with-Role-Claim-Audit-log.sln`
2. Wait for NuGet packages to restore automatically
3. If packages don't restore, right-click solution → **Restore NuGet Packages**

### Step 3: Configure Database (Optional)

**Default Configuration (No changes needed):**
The project uses **SQL Server LocalDB** by default - it works out of the box!

**Connection String Location:** `appsettings.json`

{ "ConnectionStrings": { "DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=UserManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true" } }

**Alternative Databases:**

**For SQL Server Express/Full:**
"DefaultConnection": "Server=YOUR_SERVER_NAME;Database=UserManagementDB;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=true"

**For Azure SQL:**
"DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Database=UserManagementDB;User ID=yourusername;Password=yourpassword;Encrypt=True;"

### Step 4: Run Database Migration

**Option A: Package Manager Console** (Recommended)

1. Open **Tools** → **NuGet Package Manager** → **Package Manager Console**
2. Run: Update-Database

**Option B: .NET CLI**

Open terminal in project folder and run: dotnet ef database update


**What This Does:**
- ✅ Creates `UserManagementDB` database
- ✅ Creates all ASP.NET Core Identity tables
- ✅ Creates `AuditLogs` table
- ✅ Seeds 3 default roles: **Admin**, **Manager**, **User**
- ✅ Creates default admin account with full permissions

### Step 5: Run the Application

**Visual Studio:**
- Press **F5** or click **Start Debugging**
- Or press **Ctrl+F5** for **Start Without Debugging**

**VS Code / CLI
dotnet run

The application will launch at: `https://localhost:5001` (or similar port)

### Step 6: Login as Admin

Navigate to: **https://localhost:5001/Identity/Account/Login**

**Default Admin Credentials:**
Email: admin@demo.com Password: Admin@123 Role: Admin (with ALL permissions)

### Step 7: Explore the System

After login, try these features:

1. **Manage Users** - `/Users`
   - Create new user → Assign roles → Manage claims
   - Enable/Disable accounts
   - Reset passwords

2. **Manage Roles** - `/Roles`
   - Create custom roles (e.g., "Supervisor", "Auditor")
   - View role details and assigned users
   - Assign claims to roles

3. **View Audit Logs** - `/AuditLogs`
   - See all system activities
   - Filter by action type
   - Track user changes

🎉 **Congratulations! Your system is ready.**

---

## 🗄️ Database Structure

### Standard Identity Tables
| Table | Purpose |
|-------|---------|
| `AspNetUsers` | User accounts (Id, Email, PasswordHash, etc.) |
| `AspNetRoles` | Available roles (Admin, Manager, User, custom) |
| `AspNetUserRoles` | Many-to-many: User ↔ Role assignments |
| `AspNetUserClaims` | User-specific permissions |
| `AspNetRoleClaims` | Role-based permissions (inherited by role members) |
| `AspNetUserLogins` | External login providers (reserved for v2.0) |
| `AspNetUserTokens` | Authentication tokens |

### Custom Tables
| Table | Purpose |
|-------|---------|
| `AuditLogs` | Activity tracking (UserId, Action, Timestamp, Details) |

---

## 📁 Project Structure Explained
UM-with-Role-Claim-Audit-log/ 
│ 
├── 📂 Authorization/                        ⭐ Dynamic Authorization System 
│   │ 
│   ├── ClaimBasedAuthorizationHandler.cs   # Evaluates roles + claims together 
│   │   
│   └── DynamicPolicyProvider.cs            # Auto-generates policies at runtime 
│ 
├── 📂 Controllers/                          🎮 MVC Controllers 
│   ├── AuditLogsController.cs              # View audit trail & activity logs 
│   ├── ClaimsController.cs                 # User claims management 
│   ├── HomeController.cs                   # Homepage & public pages 
│   ├── RoleClaimsController.cs             # Role claims management 
│   ├── RolesController.cs                  # Role CRUD operations 
│   └── UsersController.cs                  # User CRUD + role/claim management 
│ 
├── 📂 Data/                                 💾 Database Layer 
│   ├── ApplicationDbContext.cs             # EF Core DbContext 
│   ├── SeedData.cs                         # ✨ Seeds roles & admin (NO claims seeding!) 
│   └── Migrations/                         # Database migrations 
│       ├── 20241227000001_Initial.cs 
│       └── ApplicationDbContextModelSnapshot.cs 
│ 
├── 📂 Models/                               📊 Data Models 
│   ├── AuditLog.cs                         # Audit log entity 
│   ├── AuditLogViewModel.cs                # View model for audit logs 
│   ├── ErrorViewModel.cs                   # Error page model 
│   └── ViewModels/                         # Data transfer objects 
│       ├── AssignRoleViewModel.cs          # For role assignment 
│       ├── CreateUserViewModel.cs          # User creation form 
│       ├── EditUserViewModel.cs            # User editing form 
│       ├── ManageClaimsViewModel.cs        # User claims management 
│       ├── ManageRoleClaimsViewModel.cs    # Role claims management 
│       ├── ResetPasswordViewModel.cs       # Password reset form 
│       ├── RoleDetailsViewModel.cs         # Role details display 
│       ├── RoleViewModel.cs                # Role list display 
│       └── UserViewModel.cs                # User list display 
│ 
├── 📂 Services/                             ⭐ Business Logic & Utilities 
│   ├── AuditLogService.cs                  # Centralized audit logging 
│   └── ClaimDefinitionsService.cs          # ✨ Single source of truth for all claims 
│ 
├── 📂 Views/                                🎨 Razor Views (UI) 
│   ├── AuditLogs/ 
│   │   └── Index.cshtml                    # Audit logs list with pagination 
│   ├── Claims/ 
│   │   ├── Available.cshtml                # List all available claims 
│   │   └── Manage.cshtml                   # Manage user claims (with inheritance) 
│   ├── Home/ 
│   │   ├── Index.cshtml                    # Homepage with feature showcase 
│   │   └── Privacy.cshtml                  # Privacy policy page 
│   ├── RoleClaims/ 
│   │   └── Manage.cshtml                   # Manage role claims (with Admin warning) 
│   ├── Roles/ 
│   │   ├── Create.cshtml                   # Create new role 
│   │   ├── Delete.cshtml                   # Delete role confirmation 
│   │   ├── Details.cshtml                  # Role details with users & claims 
│   │   ├── Edit.cshtml                     # Edit role name 
│   │   └── Index.cshtml                    # Roles list 
│   ├── Shared/ 
│   │   ├── _Layout.cshtml                  # Main layout with navigation 
│   │   ├── _LoginPartial.cshtml            # Login/Logout partial view 
│   │   └── _ValidationScriptsPartial.cshtml # Client-side validation scripts 
│   └── Users/ 
│       ├── Create.cshtml                   # Create new user 
│       ├── Delete.cshtml                   # Delete user confirmation 
│       ├── Details.cshtml                  # User details with roles & claims 
│       ├── Edit.cshtml                     # Edit user & assign roles 
│       ├── Index.cshtml                    # Users list (Admin can see all) 
│       └── ResetPassword.cshtml            # Admin password reset 
│ 
├── 📂 Areas/Identity/                       🔐 ASP.NET Core Identity UI 
│   └── Pages/ 
│       └── Account/ 
│           ├── Login.cshtml                # Custom login page 
│           ├── Login.cshtml.cs             # Login page code-behind 
│           ├── Register.cshtml             # Custom registration page 
│           ├── Register.cshtml.cs          # Registration code-behind 
│           ├── Logout.cshtml               # Logout handler 
│           └── _ViewImports.cshtml         # Razor imports 
│ 
├── 📂 wwwroot/                              🌐 Static Files 
│   ├── css/ 
│   │   └── site.css                        # Custom CSS styles 
│   ├── js/ 
│   │   └── site.js                         # Custom JavaScript 
│   ├── lib/                                # Client-side libraries 
│   │   ├── bootstrap/                      # Bootstrap 5 (MIT License) 
│   │   ├── jquery/                         # jQuery (MIT License) 
│   │   └── jquery-validation/              # jQuery Validation 
│   └── favicon.ico                         # Site icon 
│
├── 📂 Properties/ 
│   └── launchSettings.json                 # Development launch profiles 
│ 
├── 📄 Program.cs                            ⚙️ Application startup & configuration 
├── 📄 appsettings.json                      🔧 Configuration (connection strings, etc.) 
├── 📄 appsettings.Development.json          🛠️ Development-specific settings 
│
├── 📄 UM-with-Role-Claim-Audit-log.csproj   📦 Project file (.NET 9, C# 13) 
├── 📄 UM-with-Role-Claim-Audit-log.sln      🗂️ Visual Studio solution file 
│ 
├── 📄 README.md                             📖 Project documentation (this file) 
├── 📄 FEATURES.md                           📋 Detailed feature documentation 
└── 📄 LICENSE.TXT                           ⚖️ Commercial license agreement 

---

## 🗂️ Key Directories Explained

### ⭐ **Authorization/** - The Heart of Dynamic System
- **DynamicPolicyProvider.cs**: Intercepts policy requests and generates them at runtime
- **ClaimBasedAuthorizationHandler.cs**: Evaluates user permissions (Admin bypass + claims check)
- **ClaimRequirement.cs**: Defines what claim is needed for authorization

### 🎮 **Controllers/** - MVC Controllers
- Handle HTTP requests and return views
- All protected with `[Authorize(Policy = "...")]` attributes
- Business logic coordinated through services

### 💾 **Data/** - Database Layer
- **ApplicationDbContext**: EF Core context with Identity tables + AuditLogs
- **SeedData**: Only seeds 3 roles (Admin/Manager/User) + admin account
- **Migrations**: Database version control

### 📊 **Models/** - Data Transfer
- **Entities**: AuditLog.cs
- **ViewModels**: Data objects for views (no business logic)

### ⭐ **Services/** - Business Logic
- **ClaimDefinitionsService**: ✨ Single source of truth for all claims
- **AuditLogService**: Centralized logging of all user actions

### 🎨 **Views/** - Razor UI
- **Claims**: Manage user-specific claims
- **RoleClaims**: Manage role claims (affects all users in role)
- **Users/Roles**: CRUD operations with authorization
- **Shared/_Layout.cshtml**: Navigation bar with role-based menu items

### 🔐 **Areas/Identity/** - Authentication UI
- Scaffolded ASP.NET Core Identity pages
- Customized Login/Register pages

---

## 🔑 Key Files Breakdown

| File | Purpose | Key Features |
|------|---------|--------------|
| **Program.cs** | App startup | - DI container setup<br/>- Dynamic policy provider registration<br/>- Identity configuration |
| **ClaimDefinitionsService.cs** | Claims catalog | - Dictionary of all available claims<br/>- Descriptions for UI<br/>- Single place to add new permissions |
| **DynamicPolicyProvider.cs** | Policy generation | - Extracts claim name from policy name<br/>- Creates policies at runtime<br/>- No hardcoded policies needed |
| **SeedData.cs** | Database seeding | - Creates default roles<br/>- Creates admin account<br/>- **Does NOT seed claims** (UI-based) |
| **_Layout.cshtml** | Master layout | - Navigation bar<br/>- Role-based menu visibility<br/>- Bootstrap 5 UI |

---

## 🚀 Architecture Highlights

### ✨ **Dynamic Authorization Flow:**
1.	Developer adds claim to ClaimDefinitionsService.cs ↓
2.	Developer uses [Authorize(Policy = "ClaimNamePolicy")] in controller ↓
3.	DynamicPolicyProvider auto-generates policy at runtime ↓
4.	Admin assigns claim to roles/users via UI ↓
5.	ClaimBasedAuthorizationHandler evaluates permissions ↓
6.	User granted/denied access

### ✨ **Zero Configuration Claims:**
- ❌ NO policy registration in Program.cs
- ❌ NO hardcoded claims in SeedData.cs
- ✅ Add claim in 1 place (ClaimDefinitionsService)
- ✅ Use in controller with [Authorize]
- ✅ Assign via UI
- ✅ Works immediately!

---

## 📊 File Count Summary

| Category | Count | Notes |
|----------|-------|-------|
| **Controllers** | 6 | MVC controllers |
| **Views** | 20+ | Razor views (.cshtml) |
| **Models** | 10+ | Entities + ViewModels |
| **Services** | 2 | Business logic layer |
| **Authorization** | 3 | Dynamic policy system |
| **Identity Pages** | 4+ | Login/Register/Logout |
| **Total C# Files** | ~40 | Excluding generated code |

---

## 🔧 Configuration Files

- **appsettings.json**: Connection strings, logging levels
- **launchSettings.json**: Development server settings
- **.csproj**: NuGet packages, target framework (.NET 9)
- **.gitignore**: Excludes bin/, obj/, *.user files

---

## 🎯 How to Navigate the Project

### For New Developers:
1. **Start here**: `Program.cs` → See how app starts
2. **Authorization**: `Authorization/` folder → Understand dynamic policies
3. **Claims**: `Services/ClaimDefinitionsService.cs` → See all permissions
4. **Controllers**: `Controllers/UsersController.cs` → See CRUD + authorization
5. **Views**: `Views/Claims/Manage.cshtml` → See UI for claim management

### For Customization:
1. **Add claims**: `Services/ClaimDefinitionsService.cs`
2. **Modify roles**: `Data/SeedData.cs`
3. **Change UI**: `Views/` and `wwwroot/css/site.css`
4. **Adjust auth logic**: `Authorization/ClaimBasedAuthorizationHandler.cs`

---

This structure showcases a clean, maintainable ASP.NET Core 9 MVC architecture with enterprise-grade authorization! 🚀

---

## 🎯 Understanding the Authorization System

### How Traditional Authorization Works:
// In Program.cs - You must register EVERY policy: builder.Services.AddAuthorization(options 
        { options.AddPolicy("CanEditUsersPolicy", policy => policy.RequireClaim("CanEditUsers", "true"));

options.AddPolicy("CanDeleteUsersPolicy", policy =>
    policy.RequireClaim("CanDeleteUsers", "true"));

// ... 20+ more policies to register manually ...
});

// Problem: Adding new permissions requires modifying Program.cs

### ✨ How Our Dynamic System Works:

**1. DynamicPolicyProvider (Authorization/DynamicPolicyProvider.cs)**
// Automatically generates policies on-the-fly: [Authorize(Policy = "CanEditUsersPolicy")]  
// ← No registration needed! public async Task<IActionResult> Edit(string id) { ... }
// Behind the scenes: 
// 1. ASP.NET Core requests "CanEditUsersPolicy" 
// 2. DynamicPolicyProvider extracts claim name: "CanEditUsers" 
// 3. Creates requirement: RequireClaim("CanEditUsers", "true") 
// 4. Returns policy automatically

**2. ClaimBasedAuthorizationHandler (Authorization/ClaimBasedAuthorizationHandler.cs)**
// Evaluates BOTH roles AND claims: protected override async Task HandleRequirementAsync( AuthorizationHandlerContext context, ClaimsAuthorizationRequirement requirement)
{ var user = context.User;
// ✅ Check 1: Is user Admin? → Allow everything
if (user.IsInRole("Admin"))
{
    context.Succeed(requirement);
    return;
}

// ✅ Check 2: Does user have the specific claim?
if (user.HasClaim(c => c.Type == requirement.ClaimType && 
                       c.Value == requirement.ClaimValue))
{
    context.Succeed(requirement);
    return;
}

// ❌ User doesn't have permission
}


**3. Using in Controllers:**
// Just add the attribute - it works! [Authorize(Policy = "CanEditUsersPolicy")] public async Task<IActionResult> Edit(string id) 
{ // Only users with "CanEditUsers" claim or Admin role can access }
[Authorize(Policy = "CanDeleteUsersPolicy")] public async Task<IActionResult> Delete(string id)
{ // Only users with "CanDeleteUsers" claim or Admin role can access }
[Authorize(Policy = "CanViewAuditLogsPolicy")] public async Task<IActionResult> Index() 
{ // Only users with "CanViewAuditLogs" claim or Admin role can access }

**4. Benefits:**
- ✅ No hardcoded policies in `Program.cs`
- ✅ Add new permissions by just creating claims
- ✅ Automatic role + claim evaluation
- ✅ Admin role bypasses all checks
- ✅ Easy to extend and maintain

---

## 🔑 Permission System Explained

### Built-in Claims (Permissions)

#### User Management:
- `CanViewUsers` - View user accounts (read-only)
- `CanEditUsers` - Create and modify user accounts
- `CanDeleteUsers` - Delete user accounts
- `CanResetPasswords` - Reset user passwords
- `CanDisableUsers` - Enable/disable accounts

#### Role Management:
- `CanViewRoles` - View roles (read-only)
- `CanManageRoles` - Full role management (includes Create, Delete)
- `CanCreateRoles` - Create new roles
- `CanDeleteRoles` - Delete roles

#### Claims Management:
- `CanViewClaims` - View user claims (read-only)
- `CanManageClaims` - Assign claims to users
- `CanAssignClaims` - Assign claims to users (same as above)
- `CanManageRoleClaims` - Assign claims to roles

#### Audit Logs:
- `CanViewAuditLogs` - View system audit logs

### Adding Custom Claims:

> **💡 Key Advantage:** With the Dynamic Policy Provider, adding new permissions is incredibly simple - **NO SeedData required!**

**Step 1: Add to `Services/ClaimDefinitionsService.cs` (Single Source of Truth):**
public static readonly Dictionary<string, string> AvailableClaims = new() 
{ 
    // Existing claims...
    // 🆕 Add your custom claims here:
    { "CanApproveInvoices", "Can approve pending invoices" },
    { "CanExportReports", "Can export system reports to Excel/PDF" },
    { "CanAccessFinancialData", "Can view financial data and analytics" },
};

**Step 2: Use in Controller:**
[Authorize(Policy = "CanApproveInvoicesPolicy")] 
// ✨ Works automatically! public async Task<IActionResult> ApproveInvoice(int id) 
{ 
    // Only users with "CanApproveInvoices" claim or Admin role can access 
    // The DynamicPolicyProvider handles everything behind the scenes 
}


**Step 3: Admin Assigns Claims via UI (No Code Changes!):**
1. Admin login → Navigate to `/Roles` → Select role (e.g., "Manager")
2. Click **"Manage Claims"** → Check "CanApproveInvoices" → Save
3. ✅ **Done!** All users in that role now have this permission

**Step 4 (Optional): Check in Views:**
@if (User.IsInRole("Admin") || User.HasClaim("CanApproveInvoices", "true")) { <button>Approve Invoice</button> }

**🎯 That's it!** The dynamic system handles everything else:
- ✅ Policy auto-generated at runtime
- ✅ Claim appears in UI automatically with description
- ✅ No database migration needed
- ✅ No application restart required
- ✅ Admin configures permissions via UI

---

## 🛠️ Customization Guide

### 1. Change Default Admin Credentials

**File:** `Data/SeedData.cs`
// Find this section: var adminEmail = "admin@demo.com"; var adminPassword = "Admin@123";
// Change to your values: var adminEmail = "your-email@company.com"; var adminPassword = "YourSecurePassword123!";

Then delete the database and run migration again, or update manually in SQL Server.

### 2. Add Custom Roles

**File:** `Data/SeedData.cs`
// Find this array: string[] roleNames = { "Admin", "Manager", "User" };
// Add your custom roles: string[] roleNames = { "Admin", "Manager", "User", "Supervisor", "Auditor", "Guest" };
Then run:dotnet ef database update

### 3. Customize Password Policy

**File:** `Program.cs`
builder.Services.AddDefaultIdentity<IdentityUser>(options => { // Email confirmation (disabled in v1.0) options.SignIn.RequireConfirmedAccount = false;
// Password requirements:
options.Password.RequiredLength = 8;          // Minimum length
options.Password.RequireDigit = true;         // Must have number
options.Password.RequireUppercase = true;     // Must have uppercase
options.Password.RequireLowercase = true;     // Must have lowercase
options.Password.RequireNonAlphanumeric = false; // Special chars optional
options.Password.RequiredUniqueChars = 4;     // Minimum unique characters
})

### 4. Add Custom Claim Descriptions

> **🎯 Best Practice:** Don't hardcode descriptions in Views! Use the centralized service instead.

**File:** `Services/ClaimDefinitionsService.cs` (Single place to define everything)
public static readonly Dictionary<string, string> AvailableClaims = new() 
{ 
    { "CanEditUsers", "Can create and modify user accounts" },
    { "CanDeleteUsers", "Can delete user accounts" },
    { "CanResetPasswords", "Can reset user passwords" },
    { "CanDisableUsers", "Can enable/disable user accounts" },
    // 🆕 Add your custom claims here:
    { "CanApproveInvoices", "Can approve pending invoices" },
    { "CanExportReports", "Can export system reports to Excel/PDF" },
    { "CanAccessFinancialData", "Can view financial data and analytics" },
};

**What happens automatically:**
- ✅ Controllers access via `ClaimDefinitionsService.AvailableClaims`
- ✅ Views display descriptions from ViewModel (populated by service)
- ✅ UI automatically shows new claims in Manage Claims pages
- ✅ **NO need to edit Views manually!**

**How the system works:**
ClaimDefinitionsService (1 place)
↓ Controllers populate ViewModels with descriptions 
↓ Views display @claim.Description automatically 
↓ ✨ Zero duplication, full sync!

### 5. Modify Audit Log Details

**File:** `Services/AuditLogService.cs`
public async Task LogAsync(string userId, string action, string details) { var auditLog = new AuditLog {
UserId = userId, Action = action, Details = details, Timestamp = DateTime.UtcNow, 
// You can change timezone here
// Add custom fields if you extend the model: 
// IpAddress = GetUserIpAddress(),
// UserAgent = GetUserAgent() };
_context.AuditLogs.Add(auditLog);
await _context.SaveChangesAsync();
}

---

## 🚀 Deployment Guide

### Deploy to IIS (Windows Server)

**1. Publish the Project:**
- Right-click project → **Publish**
- Select **Folder** → Choose output path
- Click **Publish**

**2. Update Connection String:**
Edit `appsettings.json` in published folder:
{ "ConnectionStrings": { "DefaultConnection": "Server=PRODUCTION_SERVER;Database=UserManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true" } }

**3. Run Migrations on Production Database:**
dotnet ef database update --connection "Server=PRODUCTION_SERVER;Database=UserManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true"

**4. Configure IIS:**
- Create **Application Pool** (.NET CLR Version: **No Managed Code**)
- Set Identity to account with database access
- Create **Website** pointing to published folder
- Ensure **.NET Hosting Bundle** is installed

**5. Set Permissions:**
- Give IIS user read/write access to published folder
- Configure firewall rules for database access

### Deploy to Azure App Service

**1. Create Resources:**
- Create **Azure App Service** (Windows, .NET 9)
- Create **Azure SQL Database**

**2. Configure Connection String:**
- Go to App Service → **Configuration** → **Connection strings**
- Add `DefaultConnection`:Server=tcp:yourserver.database.windows.net,1433;Database=UserManagementDB;User ID=yourusername;Password=yourpassword;Encrypt=True;

**3. Publish from Visual Studio:**
- Right-click project → **Publish**
- Select **Azure** → **Azure App Service (Windows)**
- Sign in and select your App Service
- Click **Publish**

**4. Run Migrations:**
Migrations run automatically on first start (configured in `Program.cs`).

### Deploy with Docker

**1. Create Dockerfile:**
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base WORKDIR /app EXPOSE 80 EXPOSE 443
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build WORKDIR /src COPY ["UM-with-Role-Claim-Audit-log.csproj", "./"] RUN dotnet restore COPY . . RUN dotnet build -c Release -o /app/build
FROM build AS publish RUN dotnet publish -c Release -o /app/publish
FROM base AS final WORKDIR /app COPY --from=publish /app/publish . ENTRYPOINT ["dotnet", "UM-with-Role-Claim-Audit-log.dll"]

**2. Build and Run:**
docker build -t user-management-system . docker run -p 8080:80 -e ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING" user-management-system

---

## 🐛 Troubleshooting

### ❌ "Pending model changes" error

**Solution:**
Remove all migrations
dotnet ef migrations remove
Create fresh migration
dotnet ef migrations add InitialCreate
Update database
dotnet ef database update

### ❌ Cannot login / Invalid credentials

**Check these:**
1. Verify `RequireConfirmedAccount = false` in `Program.cs`
2. Check admin user exists:SELECT * FROM AspNetUsers WHERE Email = 'admin@demo.com'
3. Ensure `EmailConfirmed = 1` for admin
4. Verify password meets requirements (6+ chars, uppercase, lowercase, digit, special char)

### ❌ Database connection error

**LocalDB Issues:**
- Ensure **SQL Server LocalDB** is installed (comes with Visual Studio)
- Try: `sqllocaldb info` in command prompt
- If not found, install from Visual Studio Installer → Individual Components → SQL Server Express LocalDB

**SQL Server Issues:**
- Verify connection string in `appsettings.json`
- Check SQL Server is running
- Ensure firewall allows connection
- Test connection in SQL Server Management Studio (SSMS)

### ❌ Migrations not running

**Solution:**
Install EF Core tools globally
dotnet tool install --global dotnet-ef
Verify installation
dotnet ef --version
Run migration
dotnet ef database update

### ❌ NuGet packages not restoring

**Solution:**
1. Right-click solution → **Restore NuGet Packages**
2. Or via CLI:dotnet restore
3. Clear NuGet cache if needed:dotnet nuget locals all --clear

### ❌ "Access Denied" when testing permissions

**Check:**
1. User has the required role or claim
2. Check user claims in database:SELECT u.Email, c.ClaimType, c.ClaimValue FROM AspNetUsers u JOIN AspNetUserClaims c ON u.Id = c.UserId WHERE u.Email = 'your-user@example.com'
3. Check role claims:SELECT r.Name, c.ClaimType, c.ClaimValue FROM AspNetRoles r JOIN AspNetRoleClaims c ON r.Id = c.RoleId

---

## 📧 Support & Updates

### Getting Help
- **Email Support:** [Your Email]
- **GitHub Issues:** [https://github.com/tantd07/UM-with-Role-Claim-Audit-log/issues](https://github.com/tantd07/UM-with-Role-Claim-Audit-log/issues)
- **Documentation:** This README + inline code comments

### License
This project is licensed under a **Commercial License** - see [LICENSE.txt](LICENSE.txt) for details.

**What You Can Do:**
- ✅ Use in unlimited personal projects
- ✅ Use in unlimited commercial projects
- ✅ Use in client projects
- ✅ Modify and customize
- ✅ Integrate into your products

**What You Cannot Do:**
- ❌ Resell this source code
- ❌ Redistribute on code marketplaces
- ❌ Share with non-purchasers
- ❌ Remove this license file

## 💰 Version & Pricing Overview

- **V1.0 ($39)** – Core authorization foundation  
  Best for internal tools, admin panels, and monolithic MVC apps.

- **V2.0 ($69 – coming soon)** – Production-ready platform  
  Recommended for SaaS, API-based systems, and frontend/mobile integrations.


### Roadmap – Version 2.0 (Planned)
This roadmap outlines the planned direction of Version 2.0.
Feature scope may be adjusted based on real-world feedback and priorities.

🔐 Authentication Enhancements (V2.0 Core)

    🔹 Email confirmation system

    🔹 Two-Factor Authentication (2FA)

    🔹 Password recovery via email

    🔹 Improved account lockout & security policies

These features focus on making the system suitable for customer-facing applications.


🎨 UI / UX Improvements (V2.0 Core)

    🔹 Refined and modernized UI design

    🔹 Improved layout consistency & usability

    🔹 Fully responsive (mobile-first) interface

    🔹 Optional dark mode

UI in v2.0 will remain clean and professional, not theme-heavy.


🌐 API & Integration Layer (V2.0 Core)

    🔹 RESTful API for user, role, and claim management

    🔹 JWT authentication support

    🔹 Swagger / OpenAPI documentation

Enables integration with SPA, mobile apps, and third-party systems.


🧪 Quality & Production Readiness (V2.0 Core)

    🔹 Unit tests & integration tests (targeting 80%+ coverage)

    🔹 Health check endpoints

    🔹 Docker & Docker Compose support

Designed for production environments and CI/CD pipelines.


🔮 Post-V2 / Future Enhancements (Not guaranteed for v2.0)

    These features are planned but may ship after v2.0, depending on demand:

    🔸 External login providers (Google, Microsoft, Facebook)

    🔸 Bulk user operations (import/export)

    🔸 Advanced audit log filtering & exports

    🔸 User activity timeline

    🔸 Session management dashboard

    🔸 Performance optimizations & caching strategies

📌 Version Positioning Summary

    V1.0 → Authorization foundation for MVC applications

    V2.0 → Production-ready platform for SaaS, API, and multi-client systems

---

## 🎉 Thank You!

Thank you for purchasing this project! Your support helps us continue improving and adding new features.

### Show Your Support:
- ⭐ **Star on GitHub** - [github.com/tantd07/UM-with-Role-Claim-Audit-log](https://github.com/tantd07/UM-with-Role-Claim-Audit-log)
- 📣 **Leave a Review** on Gumroad - Help others discover this project
- 💬 **Send Feedback** - Your suggestions shape future updates
- 🐛 **Report Issues** - Help us improve quality

### Stay Updated:
Watch the GitHub repository to get notified about v2.0 release and updates!

---

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [ASP.NET Core Authorization](https://docs.microsoft.com/aspnet/core/security/authorization)
- [Bootstrap 5 Documentation](https://getbootstrap.com/docs/5.0)

---

## 🛠️ Support Scope

Support covers:
- Setup & configuration questions
- Clarification about authorization logic

Support does NOT include:
- Custom feature development
- UI redesign
- Business-specific logic implementation

---

**Built with ❤️ by TanTran**

**Version:** 1.0.0 | **Last Updated:** December 2025

**Happy Coding!** 🚀