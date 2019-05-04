using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Narik.Common.Web.Infrastructure.Authorization.RoleBased
{
    public class NarikRoleAuthorizationHandler : AuthorizationHandler<NarikRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NarikRoleRequirement requirement)
        {
            if (!requirement.IsOverride)
            {
                var authContext = (context.Resource as AuthorizationFilterContext);
                if (authContext?.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    if (controllerActionDescriptor.MethodInfo
                        .GetCustomAttributes(inherit: true).Any(x => (x.GetType()==typeof(NarikOverrideAuthorizeAttribute)
                                                                      || (
                                                                          x.GetType() == typeof(NarikAuthorizeAttribute)
                                                                          && ((NarikAuthorizeAttribute)x).IsOverride)
                                                                      )))
                        context.Succeed(requirement);
                }
            }
            if (context.User != null)
            {
                var found = requirement.Roles.Any(r => context.User.IsInRole(r));
                if (found)
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
