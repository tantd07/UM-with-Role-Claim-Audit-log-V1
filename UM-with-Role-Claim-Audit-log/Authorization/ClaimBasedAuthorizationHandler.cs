using Microsoft.AspNetCore.Authorization;

namespace UM_with_Role_Claim_Audit_log.Authorization
{
    /// <summary>
    /// Custom authorization handler that checks if user has a specific claim
    /// or is in Admin role (Admin bypasses all checks)
    /// </summary>
    public class ClaimBasedAuthorizationHandler : AuthorizationHandler<ClaimRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ClaimRequirement requirement)
        {
            // Admin role bypasses all claim checks
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Check if user has the required claim with value "true"
            if (context.User.HasClaim(c => c.Type == requirement.ClaimType && c.Value == "true"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Requirement that specifies which claim type is needed
    /// </summary>
    public class ClaimRequirement : IAuthorizationRequirement
    {
        public string ClaimType { get; }

        public ClaimRequirement(string claimType)
        {
            ClaimType = claimType;
        }
    }
}