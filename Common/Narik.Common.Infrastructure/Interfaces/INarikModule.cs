
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.SignalR;

namespace Narik.Common.Infrastructure.Interfaces
{
    public interface INarikModule
    {
        string Key { get; }
        void Init();
    }

    public interface INarikWebModule : INarikModule
    {
        void RegisterOdataController(ODataModelBuilder builder);
        void RegisterSignalRHubs(HubRouteBuilder routes);
    }
}
