using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace UM_with_Role_Claim_Audit_log.Authorization
{
    /// <summary>
    /// Custom policy provider that dynamically creates policies based on claim names
    /// Policy names follow the pattern: "ClaimName" + "Policy"
    /// Example: "CanEditUsers" claim -> "CanEditUsersPolicy" policy
    /// </summary>
    public class DynamicPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;
        private const string PolicySuffix = "Policy";

        public DynamicPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            // Fallback to default provider for non-dynamic policies
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _fallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return _fallbackPolicyProvider.GetFallbackPolicyAsync();
        }

        /// <summary>
        /// Dynamically creates authorization policy based on policy name
        /// If policy name ends with "Policy", it extracts the claim name and creates a policy
        /// </summary>
        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Check if this is a dynamic claim-based policy
            if (policyName.EndsWith(PolicySuffix, StringComparison.OrdinalIgnoreCase))
            {
                // Extract claim name from policy name
                // Example: "CanEditUsersPolicy" -> "CanEditUsers"
                var claimType = policyName.Substring(0, policyName.Length - PolicySuffix.Length);

                // Build the policy dynamically
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new ClaimRequirement(claimType))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            // Fall back to default provider for non-dynamic policies
            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}