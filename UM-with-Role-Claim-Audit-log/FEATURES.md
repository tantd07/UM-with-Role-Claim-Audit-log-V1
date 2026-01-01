# 📋 Features Documentation

**User Management System with Dynamic Role-Based & Claims-Based Authorization**

This document provides a detailed breakdown of all features available in the User Management System v1.0 and the planned roadmap for Version 2.0.
Features listed under v2.0 are subject to change.

---

## 🔒 Security-First Architecture ⭐

This project is designed with **enterprise-grade security** from the ground up:

### Security Layers Implemented
┌─────────────────────────────────────────────────────────────────-┐ 
│  Layer 1: HTTPS/TLS Encryption          [Transport Security]     │
│  Layer 2: Security Headers               [Browser Protection]    │ 
│  Layer 3: ASP.NET Core Identity          [Authentication]        │ 
│  Layer 4: Dynamic Authorization          [Access Control]        │ 
│  Layer 5: Anti-CSRF Tokens               [Request Validation]    │ 
│  Layer 6: EF Core Parameterized Queries  [SQL Injection Shield]  │ 
│  Layer 7: Razor Auto-Encoding            [XSS Prevention]        │ 
│  Layer 8: Audit Logging                  [Compliance & Forensics]│ 
└─────────────────────────────────────────────────────────────────-┘

### OWASP Top 10 Protection Status

| Vulnerability | Protection Mechanism | Status |
|---------------|---------------------|--------|
| **A01: Broken Access Control** | Dynamic policy-based authorization + Audit logs | ✅ |
| **A02: Cryptographic Failures** | PBKDF2 password hashing + HTTPS | ✅ |
| **A03: Injection** | EF Core parameterized queries | ✅ |
| **A05: Security Misconfiguration** | Security headers + Secure defaults | ✅ |
| **A07: Authentication Failures** | Identity + Account lockout | ✅ |
| **A08: Data Integrity Failures** | Anti-forgery tokens | ✅ |

### Security Code Examples

**1. Password Security (Program.cs)**
options.Password.RequiredLength = 6; options.Password.RequireDigit = true;
options.Password.RequireUppercase = true; options.Password.RequireNonAlphanumeric = true;
// Result: Passwords hashed with PBKDF2-HMAC-SHA256 + unique salt

**2. CSRF Protection (All Views)**
<form asp-action="Create" method="post">
    @Html.AntiForgeryToken() 
    <!-- ✅ Auto-validated --> 
</form>

**3. XSS Prevention (Automatic)**
<h1>Welcome, @Model.UserName</h1>
<!-- If UserName = "<script>alert('XSS')</script>" --> <!-- Output: <script>alert('XSS')</script> ✅ Escaped -->

**4. Authorization with Audit**
[Authorize(Policy = "CanDeleteUsersPolicy")] 
// ✅ Permission check public async Task<IActionResult> Delete(string id) 
{ 
    // ✅ Prevent self-deletion 
    if (user.Id == _userManager.GetUserId(User))
        return RedirectToAction("Index");    
    await _userManager.DeleteAsync(user);
    await _auditLogService.LogAsync(...); 
    // ✅ Audit trail
}


### Production Deployment Checklist

- [x] **Security headers configured** (CSP, X-Frame-Options, etc.)
- [x] **HTTPS redirection enabled**
- [x] **Password policy enforced** (6+ chars, complexity rules)
- [x] **Anti-CSRF tokens on all forms**
- [x] **SQL injection protected** (EF Core parameterized queries)
- [x] **XSS prevented** (Razor auto-encoding)
- [x] **Audit logging active** (All actions tracked)
- [ ] **SSL certificate installed** (Your responsibility)
- [ ] **Connection strings secured** (Use Azure Key Vault/User Secrets)

> **🛡️ Security Guarantee:** This project follows Microsoft security best practices and has been designed to prevent common web vulnerabilities.
However, always conduct your own security audit before production deployment.

---

## Table of Contents

1. [Authentication Features](#authentication-features)
2. [Authorization Features](#authorization-features)
3. [User Management Features](#user-management-features)
4. [Role Management Features](#role-management-features)
5. [Claims Management Features](#claims-management-features)
6. [Audit Logging Features](#audit-logging-features)
7. [Security Features](#security-features)
8. [Technical Features](#technical-features)
9. [Upcoming Features (v2.0)](#upcoming-features-v20)

---

## 🔐 Authentication Features

### Session-Based Authentication
- **Cookie-based authentication** using ASP.NET Core Identity
- **Secure session management** with HttpOnly cookies
- **Anti-forgery token protection** on all forms
- **Automatic session expiration** after inactivity

### User Registration
- **Self-service registration** via `/Identity/Account/Register`
- **Email validation** (format checking)
- **Password strength enforcement** with configurable rules
- **Password confirmation** to prevent typos
- **Immediate account activation** (no email confirmation in v1.0)

### User Login
- **Standard email/password authentication**
- **"Remember Me" functionality** for persistent sessions
- **Return URL preservation** after login
- **Failed login attempt tracking**
- **Account lockout protection** (configurable)

### Password Management
- **Admin password reset** - Admins can reset any user's password
- **Password hashing** using Identity's secure algorithms (PBKDF2)
- **Password history** tracking via audit logs
- **Configurable password policy**:
  - Minimum length (default: 6 characters)
  - Require uppercase letters
  - Require lowercase letters
  - Require digits
  - Optional special characters
  - Minimum unique characters

### Session Management
- **Automatic logout** on browser close (without "Remember Me")
- **Manual logout** functionality
- **Session timeout** configuration
- **Concurrent session handling**

---

## 🛡️ Authorization Features

### Dynamic Policy Provider ⭐ **REVOLUTIONARY**
- **Runtime policy generation** - No hardcoded policies in `Program.cs`
- **Automatic claim extraction** from policy names
- **Convention-based policy naming** (e.g., `CanEditUsersPolicy` → `CanEditUsers` claim)
- **Zero-configuration authorization** - Just use `[Authorize(Policy = "...")]`
- **Extensible architecture** - Add new permissions without code changes

**How It Works:**
// ❌ Traditional approach (NOT used): builder.Services.AddAuthorization(options => { options.AddPolicy("CanEditUsersPolicy", policy => policy.RequireClaim("CanEditUsers", "true")); // Must register EVERY policy manually });
// ✅ Our Dynamic Approach: [Authorize(Policy = "CanEditUsersPolicy")] // Works automatically! public async Task<IActionResult> Edit(string id) { ... }
// Behind the scenes: // 1. DynamicPolicyProvider receives "CanEditUsersPolicy" // 2. Extracts claim name: "CanEditUsers" // 3. Creates ClaimRequirement("CanEditUsers") automatically // 4. ClaimBasedAuthorizationHandler evaluates the requirement

**Key Components:**

1. **DynamicPolicyProvider** (`Authorization/DynamicPolicyProvider.cs`)
   - Intercepts policy requests at runtime
   - Extracts claim name from policy name (removes "Policy" suffix)
   - Generates `ClaimRequirement` automatically
   - Falls back to default provider for non-dynamic policies

2. **ClaimBasedAuthorizationHandler** (`Authorization/ClaimBasedAuthorizationHandler.cs`)
   - Evaluates claim requirements
   - Checks if user is Admin (bypasses all checks)
   - Checks if user has the required claim with value "true"
   - Supports both direct user claims and inherited role claims

3. **ClaimRequirement** (Authorization requirement class)
   - Specifies which claim type is needed
   - Used internally by the authorization system

### Role-Based Access Control (RBAC)
- **Multiple role assignment** - Users can have multiple roles
- **Role hierarchy** - Admin role bypasses all permission checks
- **Role inheritance** - Users inherit claims from their roles
- **Built-in roles**: Admin, Manager, User
- **Custom role creation** - Create unlimited custom roles

### Claims-Based Access Control (CBAC)
- **Granular permissions** - Fine-grained access control
- **User-specific claims** - Direct permission assignment to users
- **Role-based claims** - Permissions shared by all role members
- **Claim inheritance** - Users automatically get claims from their roles
- **Visual distinction** - UI shows inherited vs. direct claims

### Hybrid Authorization
- **Combined evaluation** - Checks both roles AND claims
- **Admin bypass** - Admin role has universal access
- **Flexible permission model** - Role-based OR claim-based access
- **Authorization handler** evaluates in this order:
  1. Is user an Admin? → Grant access
  2. Does user have the specific claim? → Grant access
  3. Does user's role have the claim? → Grant access (inherited)
  4. Otherwise → Deny access

### Authorization Policies
All policies work automatically without registration:

| Policy Name | Required Claim | Purpose |
|------------|----------------|---------|
| `CanEditUsersPolicy` | `CanEditUsers` | Create/update users |
| `CanDeleteUsersPolicy` | `CanDeleteUsers` | Delete user accounts |
| `CanResetPasswordsPolicy` | `CanResetPasswords` | Reset user passwords |
| `CanDisableUsersPolicy` | `CanDisableUsers` | Enable/disable accounts |
| `CanViewRolesPolicy` | `CanViewRoles` | View roles (read-only) |
| `CanManageRolesPolicy` | `CanManageRoles` | Full role management |
| `CanCreateRolesPolicy` | `CanCreateRoles` | Create new roles |
| `CanDeleteRolesPolicy` | `CanDeleteRoles` | Delete roles |
| `CanViewClaimsPolicy` | `CanViewClaims` | View claims (read-only) |
| `CanManageClaimsPolicy` | `CanManageClaims` | Assign user claims |
| `CanAssignClaimsPolicy` | `CanAssignClaims` | Assign user claims |
| `CanManageRoleClaimsPolicy` | `CanManageRoleClaims` | Assign role claims |
| `CanViewAuditLogsPolicy` | `CanViewAuditLogs` | View audit logs |

---

## 👥 User Management Features

### User CRUD Operations
- **Create users** - Admin/authorized users can create new accounts
- **View user list** - Paginated list with search/filter
- **View user details** - See roles, claims, and activity
- **Edit user profile** - Update email and account settings
- **Delete users** - Soft or hard delete with audit trail
- **User search** - Filter by email, role, or status

### User Account Control
- **Enable/Disable accounts** - Soft account locking
- **Account status tracking** - Active/Inactive/Locked states
- **Email confirmation status** - (Reserved for v2.0)
- **Two-factor status** - (Reserved for v2.0)
- **Last login tracking** - Via audit logs

### User Details View
- **Basic information**: Email, Username, Account Status
- **Assigned roles**: Visual role badges
- **Direct claims**: User-specific permissions
- **Inherited claims**: Permissions from roles (read-only)
- **Recent activity**: Last 10 audit log entries
- **Account metadata**: Created date, last modified

### Password Reset (Admin)
- **Admin-initiated reset** - No email required
- **New password generation** - Manual or auto-generated
- **Secure password delivery** - Admin communicates to user
- **Password reset tracking** - Logged in audit trail
- **Force password change** - Option to require change on next login

### User Role Assignment
- **Multiple role assignment** - Assign/remove multiple roles at once
- **Visual role management** - Checkboxes for easy selection
- **Role change tracking** - All changes logged
- **Permission preview** - See effective permissions after role changes

---

## 🛡️ Role Management Features

### Role CRUD Operations
- **Create custom roles** - Beyond default Admin/Manager/User
- **View all roles** - List with user count and claim count
- **View role details** - See assigned users and claims
- **Edit role name** - Rename roles (updates all references)
- **Delete roles** - Remove roles from system and all users

### Role Details View
- **Role information**: Name, normalized name
- **Assigned users**: List of users with this role
- **Role claims**: Permissions for this role
- **User count**: Number of users assigned
- **Claim count**: Number of permissions attached
- **Quick actions**: Manage claims, delete role

### Default Roles
Built-in roles seeded on first run:

| Role | Description | Default Claims |
|------|-------------|----------------|
| **Admin** | Full system access | ALL (bypasses permission checks) |
| **Manager** | User and role management | CanEditUsers, CanViewRoles, CanManageClaims |
| **User** | Basic access | (None - read-only access) |

### Custom Role Creation
- **Unlimited custom roles** - Create roles like "Supervisor", "Auditor", "Guest"
- **Flexible naming** - Any alphanumeric name
- **Automatic normalization** - Handles uppercase/lowercase
- **Immediate availability** - Ready to assign after creation

---

## 🔑 Claims Management Features

### User Claims Management
- **View user claims** - See all permissions for a user
- **Add user claims** - Assign specific permissions
- **Remove user claims** - Revoke permissions
- **Claim type and value** - Flexible claim structure
- **Visual indicators**: 
  - 🟢 **Direct claim** - Assigned directly to user
  - 🔵 **Inherited claim** - From user's roles (read-only)

### Role Claims Management
- **View role claims** - See all permissions for a role
- **Add role claims** - Assign permissions to entire role
- **Remove role claims** - Revoke permissions from role
- **Batch assignment** - Multiple claims at once
- **Inheritance preview** - See which users will be affected

### Built-in Claims (Permissions)

#### User Management Claims:
- `CanEditUsers` - Create and modify user accounts
- `CanDeleteUsers` - Delete user accounts
- `CanResetPasswords` - Reset user passwords
- `CanDisableUsers` - Enable/disable accounts

#### Role Management Claims:
- `CanViewRoles` - View roles (read-only access)
- `CanManageRoles` - Full role management (CRUD)
- `CanCreateRoles` - Create new roles
- `CanDeleteRoles` - Delete roles

#### Claims Management Claims:
- `CanViewClaims` - View user claims (read-only)
- `CanManageClaims` - Assign/remove user claims
- `CanAssignClaims` - Assign claims to users
- `CanManageRoleClaims` - Assign claims to roles

#### Audit Log Claims:
- `CanViewAuditLogs` - View system audit logs

### Custom Claims Support ⭐ **DYNAMIC SYSTEM**
- **Add any custom claim** - Extend with business-specific permissions
- **No code changes required** - Just add claim via UI and use in controller
- **Automatic policy generation** - Dynamic system handles everything
- **Claim descriptions** - Add friendly descriptions in UI (optional)

**Example Custom Claims:**
// These are examples - you can add ANY claim name following the convention new Claim("CanApproveInvoices", "true") new Claim("CanExportReports", "true") new Claim("CanAccessFinancialData", "true") new Claim("CanManageInventory", "true") new Claim("CanViewAnalytics", "true")

**Naming Convention:**
- ✅ Format: `Can[Action][Entity]` (e.g., `CanApproveInvoices`)
- ✅ Policy name: Add "Policy" suffix (e.g., `CanApproveInvoicesPolicy`)
- ✅ Value: Always `"true"`
- ✅ Use PascalCase

### Claims Inheritance
- **Automatic inheritance** - Users get claims from their roles
- **Combined evaluation** - User claims + role claims
- **Read-only inherited claims** - Cannot be removed individually
- **Visual distinction** - Different colors/icons for claim sources

---

## 📝 Audit Logging Features

### Comprehensive Activity Tracking
Every system action is logged with:
- **User ID** - Who performed the action
- **Action type** - What was done
- **Details** - Specific information about the action
- **Timestamp** - When it occurred (UTC)
- **IP address** - (Can be extended)
- **User agent** - (Can be extended)

### Logged Events

#### User Events:
- User created (email, roles)
- User updated (changed fields)
- User deleted (permanent removal)
- User enabled/disabled
- Password reset (by whom)
- User login (successful/failed)
- User logout

#### Role Events:
- Role created (name)
- Role assigned to user
- Role removed from user
- Role deleted (affected users)
- Role renamed

#### Claims Events:
- Claim added to user
- Claim removed from user
- Claim added to role
- Claim removed from role

### Audit Log Viewing
- **Paginated list** - Navigate through logs easily
- **Date/time display** - Local timezone conversion
- **User identification** - Email or "Unknown" for deleted users
- **Action categorization** - Color-coded by type
- **Detail expansion** - Click to see full details
- **Search functionality** - Filter by action type or user
- **Export capability** - (Can be extended for CSV/Excel)

### Audit Log Details
Each log entry contains:
{ "Id": "unique-guid", "UserId": "user-guid", "Action": "User Created", "Details": "Created user: john@example.com | Roles: Manager", "Timestamp": "2025-12-27T10:30:00Z" }

### Retention and Compliance
- **Permanent storage** - Logs never deleted by default
- **GDPR compliance** - Can be extended to anonymize deleted user data
- **Audit trail integrity** - No edit capability (append-only)
- **Compliance reporting** - Export logs for auditors

---

## 🔒 Security Features

### Password Security
- **Secure hashing** - PBKDF2 with random salt (Identity default)
- **No plain-text storage** - Passwords never stored unencrypted
- **Password policy enforcement**:
  - Minimum length: 6 characters (configurable)
  - Require uppercase: Yes
  - Require lowercase: Yes
  - Require digit: Yes
  - Require special character: Optional
  - Unique characters: 4 minimum
- **Password history** - Track changes via audit logs

### Session Security
- **HttpOnly cookies** - Prevents XSS attacks
- **Secure cookie flag** - HTTPS enforcement in production
- **SameSite cookie policy** - CSRF protection
- **Anti-forgery tokens** - On all POST forms
- **Session timeout** - Configurable idle timeout
- **Concurrent session control** - Limit active sessions

### Authorization Security
- **Least privilege principle** - Default User role has minimal access
- **Admin separation** - Admin role clearly distinguished
- **Permission verification** - Every action checked
- **Claim-based granularity** - Fine-grained access control
- **Role change auditing** - All permission changes logged

### Data Protection
- **SQL injection protection** - Entity Framework parameterized queries
- **XSS protection** - Razor automatic HTML encoding
- **CSRF protection** - Anti-forgery tokens
- **HTTPS enforcement** - Redirect to HTTPS in production
- **Secure connection strings** - Encrypted configuration

### Input Validation
- **Model validation** - Server-side validation on all inputs
- **Client-side validation** - jQuery unobtrusive validation
- **Email format validation** - RegEx pattern matching
- **Required field enforcement** - Data annotations
- **String length limits** - Prevent buffer overflow

---

## 🛠️ Technical Features

### Framework & Architecture
- **ASP.NET Core 9 MVC** - Latest stable framework
- **Entity Framework Core 9** - Modern ORM
- **ASP.NET Core Identity** - Built-in authentication
- **Razor Pages** - For Identity UI
- **Bootstrap 5** - Responsive UI framework
- **jQuery** - Client-side interactions

### Database
- **SQL Server LocalDB** - Default development database
- **SQL Server Express/Full** - Supported
- **Azure SQL Database** - Supported
- **Code-first migrations** - Database version control
- **Automatic migrations** - Can run on app startup
- **Seed data** - Pre-populated roles and admin

### Performance
- **Async/await pattern** - Non-blocking operations
- **Eager loading** - Include related data efficiently
- **Pagination** - Limit database queries
- **Indexed queries** - Identity tables pre-optimized
- **Connection pooling** - Efficient database connections

### Dependency Injection
- **Service lifetime management** - Scoped, Transient, Singleton
- **Injected services**:
  - `UserManager<IdentityUser>`
  - `RoleManager<IdentityRole>`
  - `SignInManager<IdentityUser>`
  - `ApplicationDbContext`
  - `AuditLogService`
  - `IAuthorizationPolicyProvider`
  - `IAuthorizationHandler`

### Configuration
- **appsettings.json** - Environment-specific settings
- **Connection string management** - Centralized configuration
- **User secrets** - Development secrets (can be added)
- **Environment variables** - Production configuration

### Error Handling
- **Developer exception page** - Detailed errors in development
- **Custom error pages** - User-friendly errors in production
- **Model state validation** - Form error display
- **Try-catch blocks** - Graceful error handling
- **Logging infrastructure** - Built-in .NET logging

### Code Organization
- **MVC pattern** - Clear separation of concerns
- **ViewModels** - Data transfer objects
- **Service layer** - Business logic separation
- **Repository pattern** - (Can be extended)
- **Dependency inversion** - Interface-based design

---

## 🚀 Upcoming Features (v2.0)

### Authentication Enhancements
- ✅ **Email confirmation system** - Verify email before account activation
- ✅ **Two-Factor Authentication (2FA)** - Authenticator app support
- ✅ **SMS verification** - Phone number confirmation
- ✅ **External login providers** - Google, Microsoft, Facebook, GitHub
- ✅ **Password recovery via email** - Self-service password reset
- ✅ **Account lockout improvements** - Configurable lockout policies
- ✅ **Magic link login** - Passwordless authentication

### UI/UX Improvements
- ✅ **Modern interface design** - Professional, polished UI
- ✅ **Smooth animations** - Page transitions and interactions
- ✅ **Dark mode support** - Theme toggle
- ✅ **Responsive mobile-first design** - Optimized for all devices
- ✅ **Dashboard with analytics** - User activity charts
- ✅ **Real-time notifications** - SignalR integration
- ✅ **Toast notifications** - Non-intrusive feedback

### Additional Features
- ✅ **Bulk user operations** - Import/export/update multiple users
- ✅ **User import/export** - CSV and Excel support
- ✅ **Advanced audit log filtering** - Date range, action type, user
- ✅ **User activity timeline** - Visual representation of actions
- ✅ **Session management** - View and terminate active sessions
- ✅ **User groups** - Group-based permissions
- ✅ **Permission templates** - Pre-defined permission sets

### Technical Enhancements
- ✅ **Docker Compose support** - Containerized deployment
- ✅ **Health check endpoints** - Monitoring and diagnostics
- ✅ **RESTful API** - API endpoints for integrations
- ✅ **Unit tests** - Comprehensive test coverage
- ✅ **Integration tests** - End-to-end testing
- ✅ **Performance optimizations** - Caching, query optimization
- ✅ **API documentation** - Swagger/OpenAPI
- ✅ **Rate limiting** - API throttling

---

## 📊 Feature Comparison Matrix

| Feature Category | v1.0 | v2.0 (Planned) |
|-----------------|------|----------------|
| **Authentication** | | |
| Email/Password Login | ✅ | ✅ |
| Session Management | ✅ | ✅ |
| Email Confirmation | ❌ | ✅ |
| Two-Factor Auth (2FA) | ❌ | ✅ |
| External Logins | ❌ | ✅ |
| Password Recovery | ⚠️ Admin Only | ✅ Self-Service |
| **Authorization** | | |
| Role-Based Access | ✅ | ✅ |
| Claims-Based Access | ✅ | ✅ |
| Dynamic Policies | ✅ | ✅ |
| Group Permissions | ❌ | ✅ |
| Permission Templates | ❌ | ✅ |
| **User Management** | | |
| CRUD Operations | ✅ | ✅ |
| Bulk Operations | ❌ | ✅ |
| Import/Export | ❌ | ✅ |
| Activity Timeline | ⚠️ Audit Logs | ✅ Visual |
| Session Management | ❌ | ✅ |
| **UI/UX** | | |
| Bootstrap 5 UI | ✅ Basic | ✅ Enhanced |
| Dark Mode | ❌ | ✅ |
| Animations | ⚠️ Minimal | ✅ Smooth |
| Mobile Responsive | ✅ | ✅ Enhanced |
| Dashboard | ❌ | ✅ |
| **Technical** | | |
| REST API | ❌ | ✅ |
| Docker Support | ⚠️ Manual | ✅ Compose |
| Health Checks | ❌ | ✅ |
| Unit Tests | ❌ | ✅ |
| API Documentation | ❌ | ✅ |

---

## 🎯 Feature Access Matrix

### By Role

| Feature | Admin | Manager | User |
|---------|-------|---------|------|
| **User Management** | | | |
| View Users | ✅ | ✅ | ❌ |
| Create Users | ✅ | ✅ | ❌ |
| Edit Users | ✅ | ✅ | ❌ |
| Delete Users | ✅ | ❌ | ❌ |
| Reset Passwords | ✅ | ❌ | ❌ |
| Enable/Disable Accounts | ✅ | ❌ | ❌ |
| **Role Management** | | | |
| View Roles | ✅ | ✅ | ❌ |
| Create Roles | ✅ | ❌ | ❌ |
| Delete Roles | ✅ | ❌ | ❌ |
| Assign Roles | ✅ | ✅ | ❌ |
| **Claims Management** | | | |
| View Claims | ✅ | ✅ | ❌ |
| Manage User Claims | ✅ | ✅ | ❌ |
| Manage Role Claims | ✅ | ❌ | ❌ |
| **Audit Logs** | | | |
| View Audit Logs | ✅ | ⚠️ | ❌ |

✅ Full Access | ⚠️ With Claim | ❌ No Access

---

## 📖 Feature Usage Examples

### Example 1: Creating a User with Custom Role
1. Navigate to `/Users`
2. Click **"Create New User"**
3. Enter email: `employee@company.com`
4. Set password: `Employee@123`
5. Select roles: **["Manager"]**
6. Click **"Create"**
7. System logs: **"User Created"** in audit logs

### Example 2: Assigning Custom Claims
1. Navigate to user details
2. Click **"Manage Claims"**
3. Add claims:
   - **Claim Type:** `CanApproveInvoices`, **Value:** `true`
   - **Claim Type:** `CanViewReports`, **Value:** `true`
4. Click **"Save"**
5. System logs: **"Claim Added to User"** (for each claim)

### Example 3: Creating Role with Permissions
1. Navigate to `/Roles`
2. Click **"Create New Role"**
3. Enter name: **"Supervisor"**
4. Click **"Create"**
5. Navigate to role details
6. Click **"Manage Claims"**
7. Assign claims: `CanEditUsers`, `CanViewRoles`
8. All users with **"Supervisor"** role inherit these claims automatically

### Example 4: Viewing Audit Trail
1. Navigate to `/AuditLogs`
2. View chronological list of all actions
3. Filter by action type: **"User Created"**
4. See: Who created which users and when
5. Click details to see full information

---

## 💡 Feature Extension Guide

### ⭐ Adding a Custom Permission (DYNAMIC SYSTEM)

**With the Dynamic Authorization System, adding custom permissions requires only 2 steps:**

#### Step 1: Add to ClaimDefinitionsService (Single Source of Truth)

**File:** `Services/ClaimDefinitionsService.cs`
public static readonly Dictionary<string, string> AvailableClaims = new() 
{ 
    // Existing built-in claims...
    // 🆕 Add your custom claims here:
    { "CanApproveInvoices", "Can approve pending invoices" },
    { "CanExportReports", "Can export system reports to Excel/PDF" },
    { "CanAccessFinancialData", "Can view financial data and analytics" },
    { "CanManageInventory", "Can manage product inventory" },
};

**What happens automatically:**
- ✅ Controllers access via `ClaimDefinitionsService.AvailableClaims`
- ✅ Views display descriptions from ViewModel (populated by service)
- ✅ UI automatically shows new claims in `/Claims/Manage` and `/RoleClaims/Manage`
- ✅ **NO need to edit Views manually!**

#### Step 2: Use in Controller
// Just add the [Authorize] attribute with your policy name [Authorize(Policy = "CanApproveInvoicesPolicy")] 
// ✨ Works automatically! public async Task<IActionResult> ApproveInvoice(int id) 
{ 
    // ✨ That's it! The DynamicPolicyProvider handles everything: 
    // 1. Extracts "CanApproveInvoices" from policy name 
    // 2. Creates ClaimRequirement automatically 
    // 3. Checks if user has the claim
    var invoice = await _context.Invoices.FindAsync(id);
    if (invoice == null) return NotFound();
        invoice.Status = "Approved";
    invoice.ApprovedBy = _userManager.GetUserId(User);
    invoice.ApprovedDate = DateTime.UtcNow;
    await _context.SaveChangesAsync();
    await _auditLogService.LogAsync(
        _userManager.GetUserId(User),
        "Invoice Approved",
        $"Approved invoice #{id}"
    );
    return RedirectToAction("Index");
}


#### Step 3: Admin Assigns Claims via UI (No Code Changes!)

**Option A: Assign to User**
1. Admin login → Navigate to `/Users` → Select user → **Details**
2. Click **"Manage Claims"**
3. ✨ **The new claim appears automatically** with its description!
4. Check the claim → Click **"Save"**

**Option B: Assign to Role (Recommended for multiple users)**
1. Navigate to `/Roles` → Select role (e.g., Manager) → **Details**
2. Click **"Manage Claims"**
3. ✨ **The new claim appears automatically** with its description!
4. Check the claim → Click **"Save"**
5. ✨ All users in that role now have this permission!

#### Step 4 (Optional): Check in Views
@if (User.IsInRole("Admin") || User.HasClaim("CanApproveInvoices", "true"))
{ 
    <form asp-action="ApproveInvoice" asp-route-id="@Model.Id" method="post"> 
    @Html.AntiForgeryToken() <button type="submit" class="btn btn-success"> 
    <i class="bi bi-check-circle"></i> Approve Invoice </button> </form> 
}

---

### 🎯 Complete Workflow Summary

**What You DON'T Need:**
- ❌ Edit Views manually (descriptions auto-sync from service)
- ❌ Register policy in `Program.cs` → Handled automatically
- ❌ Add to `SeedData.cs` → Optional, only for default seeding
- ❌ Restart application → Claims take effect immediately via UI
- ❌ Database migration → Uses existing Identity tables

**Time Savings:**
- ❌ Traditional: 10-15 minutes per new permission
- ✅ Dynamic System: **30 seconds** per new permission

---

### ⚠️ About SeedData.cs (Optional Seeding)

**You do NOT need to modify `SeedData.cs` unless:**
- ✅ You want to **seed default claims** for Admin/Manager roles on fresh database installations
- ✅ You want **demo users** to have specific permissions automatically
- ✅ You're distributing the system with pre-configured permissions

**Example (Optional - NOT required for Dynamic System to work):**
// In Data/SeedData.cs - Only if you want default seeding if (roleName == "Manager") 
{ 
    var managerClaims = new List<Claim> 
    { new Claim("CanViewUsers", "true"), new Claim("CanEditUsers", "true"), new Claim("CanViewRoles", "true"),
    // Optional: Add your custom claims for default seeding
    new Claim("CanApproveInvoices", "true"),
    new Claim("CanExportReports", "true")
    };
    foreach (var claim in managerClaims)
    {
        if (!(await roleManager.GetClaimsAsync(managerRole)).Any(c => c.Type == claim.Type))
        {
            await roleManager.AddClaimAsync(managerRole, claim);
        }
    }
}

**Important:** Modifying `SeedData.cs` is **NOT required** for the Dynamic Authorization System to work. Claims can be added via UI anytime after deployment!

---

### Creating a Custom Role

#### Step 1: Create Role via UI (Recommended)

1. Navigate to `/Roles`
2. Click **"Create New Role"**
3. Enter name: `Auditor`, `Supervisor`, `Guest`, etc.
4. Click **"Create"**
5. Navigate to role details
6. Click **"Manage Claims"** → Assign permissions from the full list (auto-populated from `ClaimDefinitionsService`)

#### Step 2 (Optional): Seed Role in SeedData.cs

Only if you want the role to exist on fresh database installations:
// In Data/SeedData.cs string[] roleNames = { "Admin", "Manager", "User", "Auditor",     
// ← Add your custom role "Supervisor"   
// ← Add another custom role };
// Optional: Assign default claims for this role if (roleName == "Auditor") 
  { 
       await roleManager.AddClaimAsync(role, new Claim("CanViewAuditLogs", "true")); 
       await roleManager.AddClaimAsync(role, new Claim("CanViewUsers", "true")); 
  }

---

### 🎯 System Architecture Benefits
ClaimDefinitionsService (Single Source of Truth) 
↓ Controllers populate ViewModels with descriptions 
↓ Views display @claim.Description automatically 
↓ ✨ Zero duplication, full auto-sync!

**Key Advantages:**
- 🎯 **Single Place to Define Claims** - `ClaimDefinitionsService.cs` only
- ⚡ **Instant UI Sync** - Descriptions auto-populate in Manage Claims pages
- 🔧 **Zero Maintenance** - Add once, works everywhere
- 📊 **Type Safety** - Claims defined in C# with IntelliSense support
- 🚀 **Infinite Scalability** - Add unlimited claims without code changes

---

### ⚠️ About SeedData.cs

**You do NOT need to add claims to `SeedData.cs` unless:**
- ✅ You want to **seed default claims** for Admin/Manager roles on database initialization
- ✅ You want **demo users** to have specific permissions automatically
- ✅ You're setting up a **fresh installation** with pre-configured permissions

**Example (Optional):**
// Only if you want to seed default claims for Admin role var adminClaims = new List<Claim>
{
    // Existing built-in claims new Claim("CanEditUsers", "true"), new Claim("CanDeleteUsers", "true"),
    // Add your custom claims (optional - for default seeding only)
    new Claim("CanApproveInvoices", "true"),
    new Claim("CanExportReports", "true")
};
foreach (var claim in adminClaims) 
{ 
    if (!(await roleManager.GetClaimsAsync(adminRole)).Any(c => c.Type == claim.Type))
        { await roleManager.AddClaimAsync(adminRole, claim); } 
}

**Important:** Modifying `SeedData.cs` is **NOT required** for the Dynamic Authorization System to work. You can add claims directly via the UI anytime!

---

### Creating a Custom Role

#### Step 1: Create Role via UI (Recommended)

1. Navigate to `/Roles`
2. Click **"Create New Role"**
3. Enter name: `Auditor`, `Supervisor`, `Guest`, etc.
4. Click **"Create"**
5. Navigate to role details
6. Click **"Manage Claims"** to assign permissions

#### Step 2: (Optional) Seed Role in SeedData.cs

Only if you want the role to exist on fresh database installations:
// In Data/SeedData.cs string[] roleNames = { "Admin", "Manager", "User", "Auditor",
// ← Add your custom role "Supervisor"  
// ← Add another custom role };
// Assign default claims for this role if (roleName == "Auditor") 
{ 
    await roleManager.AddClaimAsync(role, new Claim("CanViewAuditLogs", "true")); 
    await roleManager.AddClaimAsync(role, new Claim("CanViewUsers", "true")); 
}

---

### ✅ Complete Workflow Summary
1.	Write controller action with [Authorize(Policy = "ClaimNamePolicy")] ↓
2.	DynamicPolicyProvider automatically generates policy at runtime ↓
3.	Assign claim to User/Role via UI (/Users or /Roles) ↓
4.	ClaimBasedAuthorizationHandler evaluates permission ↓
5.	Done! ✨ No restart, no code deployment needed!

**What You DON'T Need:**
- ❌ Register policy in `Program.cs` → **Handled automatically**
- ❌ Add to `SeedData.cs` → **Optional, only for defaults**
- ❌ Restart application → **Claims take effect immediately via UI**
- ❌ Database migration → **Uses existing Identity tables**

---

## 🎯 Naming Conventions

### Claim Naming Convention
- ✅ **Format:** `Can[Action][Entity]` (PascalCase)
- ✅ **Value:** Always `"true"`
- ✅ **Examples:**
  - `CanEditUsers`
  - `CanApproveInvoices`
  - `CanExportReports`
  - `CanManageInventory`
  - `CanAccessFinancialData`

### Policy Naming Convention
- ✅ **Format:** `[ClaimName]Policy`
- ✅ **Examples:**
  - `CanEditUsersPolicy`
  - `CanApproveInvoicesPolicy`
  - `CanExportReportsPolicy`

### How They Connect
// Claim in database: Type: "CanApproveInvoices", Value: "true"
// Policy in controller: [Authorize(Policy = "CanApproveInvoicesPolicy")]
// Behind the scenes:
// "CanApproveInvoicesPolicy" → Extract "CanApproveInvoices" 
// Check if user has claim "CanApproveInvoices" = "true"

---

---

## 🏗️ Architecture Overview

### System Components
┌─────────────────────────────────────────────────────────────────┐ 
│                     ASP.NET Core Pipeline                       │ 
└─────────────────────────────────────────────────────────────────┘ 
↓
┌─────────────────────────────────────────────────────────────────┐ 
│  [Authorize(Policy = "CanEditUsersPolicy")] ← Controller Action │ 
└─────────────────────────────────────────────────────────────────┘ 
↓ 
┌─────────────────────────────────────────────────────────────────┐ 
│         DynamicPolicyProvider (IAuthorizationPolicyProvider)    │ 
│  • Receives policy name: "CanEditUsersPolicy"                   │ 
│  • Extracts claim: "CanEditUsers"                               │ 
│  • Creates: ClaimRequirement("CanEditUsers")                    │ 
└─────────────────────────────────────────────────────────────────┘ 
↓ 
┌─────────────────────────────────────────────────────────────────┐ 
│    ClaimBasedAuthorizationHandler (IAuthorizationHandler)       │ 
│  1. Check: User.IsInRole("Admin") → ✅ Allow all                │ 
│  2. Check: User.HasClaim("CanEditUsers", "true") → ✅ Allow     │ 
│  3. Check: User's roles have claim → ✅ Allow (inherited)       │  
│  4. Otherwise → ❌ Deny                                         │  
└─────────────────────────────────────────────────────────────────┘ 
↓ 
✅ Access Granted / ❌ Access Denied

### Key Files & Responsibilities

| File | Responsibility | Key Methods |
|------|----------------|-------------|
| **DynamicPolicyProvider.cs** | Policy generation at runtime | `GetPolicyAsync(policyName)` |
| **ClaimBasedAuthorizationHandler.cs** | Permission evaluation | `HandleRequirementAsync(context, requirement)` |
| **ClaimRequirement.cs** | Authorization requirement | Stores required claim type |
| **ClaimDefinitionsService.cs** | ✨ Centralized claim catalog | `AvailableClaims` dictionary |
| **Program.cs** | DI registration | Registers `DynamicPolicyProvider` + `ClaimBasedAuthorizationHandler` |

---

## 🎯 Data Flow: From Code to UI

### Adding a New Permission (End-to-End)
Step 1: Developer adds to ClaimDefinitionsService.cs { "CanApproveInvoices", "Can approve pending invoices" } 
↓ Step 2: Developer uses in Controller [Authorize(Policy = "CanApproveInvoicesPolicy")] 
↓ Step 3: DynamicPolicyProvider auto-generates policy 
↓ Step 4: ClaimsController.Manage() GET action
    • Reads ClaimDefinitionsService.AvailableClaims 
    • Populates ManageClaimsViewModel with descriptions 
↓ Step 5: Views/Claims/Manage.cshtml renders 
    • Displays claim name: "CanApproveInvoices" 
    • Displays description: "Can approve pending invoices" (from @claim.Description) 
    • Shows checkbox for assignment 
↓ Step 6: Admin checks claim → Saves 
    • POST to ClaimsController.Manage() 
    • Adds claim to AspNetUserClaims or AspNetRoleClaims table 
↓ Step 7: User attempts to access protected action 
    • ClaimBasedAuthorizationHandler checks if user has claim 
    • ✅ Access granted if claim exists

---

## 🔧 Configuration in Program.cs
// Register Dynamic Authorization System builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicPolicyProvider>(); 
   builder.Services.AddSingleton<IAuthorizationHandler, ClaimBasedAuthorizationHandler>();
// What this does: 
// 1. Replaces default policy provider with DynamicPolicyProvider 
// 2. Registers ClaimBasedAuthorizationHandler for claim evaluation 
// 3. Enables automatic policy generation at runtime

**Key Benefits:**
- ✅ Zero hardcoded policies
- ✅ Convention-based naming
- ✅ Infinite extensibility
- ✅ No application restart needed for new claims
- ✅ UI-based permission management

## 📞 Support & Feedback

For questions about specific features or feature requests:
- **Email**: tantran7183@gmail.com
- **GitHub Issues**: https://github.com/tantd07/UM-with-Role-Claim-Audit-log/issues
- **Documentation**: See README.md for setup and troubleshooting

---

**Built with ❤️ by TanTran**

**Version:** 1.0.0 | **Last Updated:** December 2025

**Feature requests for v2.0 are welcome!** 🚀

---

## 📚 Additional Notes

### Why is this system "Revolutionary"?

Traditional ASP.NET Core authorization requires you to register every single policy in `Program.cs`:
// Traditional approach - tedious and error-prone builder.Services.AddAuthorization(options => { options.AddPolicy("CanEditUsers", p => p.RequireClaim("CanEditUsers", "true")); options.AddPolicy("CanDeleteUsers", p => p.RequireClaim("CanDeleteUsers", "true")); options.AddPolicy("CanViewRoles", p => p.RequireClaim("CanViewRoles", "true")); 
// ... 20+ more policies manually registered });

**Our Dynamic System eliminates this completely:**
- ✅ Zero policy registration
- ✅ Convention-based automatic generation
- ✅ Add new permissions without touching startup code
- ✅ Truly dynamic and extensible

### Technical Implementation

1. **DynamicPolicyProvider** implements `IAuthorizationPolicyProvider`
2. ASP.NET Core calls `GetPolicyAsync(policyName)` when it sees `[Authorize(Policy = "...")]`
3. If policy name ends with "Policy", extract claim name and create requirement
4. **ClaimBasedAuthorizationHandler** implements `AuthorizationHandler<ClaimRequirement>`
5. Checks Admin role first (bypass), then user claims, then inherited role claims

This architecture allows infinite scalability without code changes! 🚀
