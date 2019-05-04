using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Narik.Common.Web.Infrastructure.Authorization.RoleBased
{
    public class NarikRoleRequirement : IAuthorizationRequirement
    {
        public NarikRoleRequirement(IEnumerable<string> roles, bool isOverride)
        {
            Roles = roles;
            IsOverride = isOverride;
        }

        public bool IsOverride { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
