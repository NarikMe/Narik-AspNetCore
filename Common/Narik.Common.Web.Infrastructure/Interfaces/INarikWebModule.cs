using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Routing;
using Narik.Common.Infrastructure.Interfaces;

namespace Narik.Common.Web.Infrastructure.Interfaces
{
    public interface INarikWebModule : INarikModule
    {
        void RegisterOdataController(ODataModelBuilder builder);
        void RegisterSignalRHubs(IEndpointRouteBuilder configure);

    }
}
