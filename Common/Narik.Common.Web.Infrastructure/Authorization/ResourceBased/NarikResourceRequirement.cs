using Microsoft.AspNetCore.Authorization;

namespace Narik.Common.Web.Infrastructure.Authorization.ResourceBased
{
    public class NarikResourceRequirement : IAuthorizationRequirement
    {
        public NarikResourceRequirement(string resources, bool isOverride)
        {
            Resources = resources;
            IsOverride = isOverride;
        }

        public bool IsOverride { get; set; }
        public string  Resources { get; set; }
    }
}
