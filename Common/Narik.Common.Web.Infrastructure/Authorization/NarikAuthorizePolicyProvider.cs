using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Narik.Common.Web.Infrastructure.Authorization.ResourceBased;
using Narik.Common.Web.Infrastructure.Authorization.RoleBased;

namespace Narik.Common.Web.Infrastructure.Authorization
{
    internal class NarikAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
       
       
        public NarikAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();


        
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(NarikAuthorizeAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var prefixLen = policyName.StartsWith(NarikAuthorizeAttribute.PolicyOverridePrefix,
                    StringComparison.OrdinalIgnoreCase)
                    ? NarikAuthorizeAttribute.PolicyOverridePrefix.Length
                    : NarikAuthorizeAttribute.PolicyPrefix.Length;
                AuthorizationPolicyBuilder policyBuilder = new AuthorizationPolicyBuilder();

                var roles = policyName.Substring(prefixLen);
                var rolesSplit = roles?.Split(',');
                if (rolesSplit.Any())
                {
                    var trimmedRolesSplit = rolesSplit.Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.Trim());
                    policyBuilder.AddRequirements(new NarikRoleRequirement(trimmedRolesSplit,
                        policyName.StartsWith(NarikAuthorizeAttribute.PolicyOverridePrefix,
                            StringComparison.OrdinalIgnoreCase)));
                }
                return Task.FromResult(policyBuilder.Build());
            }
            if (policyName.StartsWith(NarikResourceAuthorizeAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var prefixLen = policyName.StartsWith(NarikResourceAuthorizeAttribute.PolicyOverridePrefix,
                    StringComparison.OrdinalIgnoreCase)
                    ? NarikResourceAuthorizeAttribute.PolicyOverridePrefix.Length
                    : NarikResourceAuthorizeAttribute.PolicyPrefix.Length;

                var resources = policyName.Substring(prefixLen);
                AuthorizationPolicyBuilder policyBuilder = new AuthorizationPolicyBuilder();
                if (!string.IsNullOrEmpty(resources))
                {
                    policyBuilder.AddRequirements(new NarikResourceRequirement(resources,
                        policyName.StartsWith(NarikResourceAuthorizeAttribute.PolicyOverridePrefix,
                            StringComparison.OrdinalIgnoreCase)));
                }
                return Task.FromResult(policyBuilder.Build());
            }
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return FallbackPolicyProvider.GetFallbackPolicyAsync();
        }
    }
}