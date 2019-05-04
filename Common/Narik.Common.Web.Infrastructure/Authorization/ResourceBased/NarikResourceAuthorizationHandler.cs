using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Narik.Common.Services.Core;

namespace Narik.Common.Web.Infrastructure.Authorization.ResourceBased
{
    public class NarikResourceAuthorizationHandler : AuthorizationHandler<NarikResourceRequirement>
    {

        private readonly IResourceAccessService _resourceAccessService;
        public NarikResourceAuthorizationHandler(IResourceAccessService resourceAccessService)
        {
            _resourceAccessService = resourceAccessService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NarikResourceRequirement requirement)
        {
            if (!requirement.IsOverride)
            {
                var authContext = (context.Resource as AuthorizationFilterContext);
                if (authContext?.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    if (controllerActionDescriptor.MethodInfo
                        .GetCustomAttributes(inherit: true).Any(x => (x.GetType() == typeof(NarikResourceOverrideAuthorizeAttribute)
                                                                      || (
                                                                          x.GetType() == typeof(NarikResourceAuthorizeAttribute)
                                                                          && ((NarikResourceAuthorizeAttribute)x).IsOverride)
                            )))
                        context.Succeed(requirement);
                }
            }
            if (context.User != null)
            {
                if (_resourceAccessService.HasAccess(context.User.FindFirst("UserId").Value, requirement.Resources))
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
