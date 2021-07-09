
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Narik.Common.Services.Core;
using Narik.Common.Shared.Models;
using Narik.Modules.Runtime.Hubs;
using Narik.Modules.Runtime.Services;
using Unity;
using Microsoft.AspNetCore.Builder;
using Microsoft.OData.ModelBuilder;
using Unity.Lifetime;
using Narik.Common.Web.Infrastructure.Interfaces;

namespace Narik.Modules.Runtime
{
    public class RuntimeModule : INarikWebModule
    {
        private readonly IUnityContainer _unityContainer;

        public const string KEY = "Runtime";
        public string Key => KEY;

        public RuntimeModule(IUnityContainer container, NarikModulesConfig config)
        {
            _unityContainer = container;
            container.RegisterType<IUserStore<ApplicationUser>, NarikUserStore>(new ContainerControlledLifetimeManager());
        }
        public void Init()
        {
            var config = _unityContainer.Resolve<NarikModulesConfig>();
            if (config.AuthenticationMode == "Jwt")
                _unityContainer.RegisterType<ILoginService, NarikJwtLoginService>();
            else
                _unityContainer.RegisterType<ILoginService, NarikDefaultLoginService>();

            _unityContainer.RegisterType<IEventAggregator, NarikEventAggregator>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<ICacheService, NarikMemoryCacheService>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IAppStorage, NarikAppStororage>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<ISessionStorage, NarikSessionStorage>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<ILocalizationService, NarikLocalizationService>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IEntityUpdatePushService, NarikEntityUpdatePushService>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IUserEnvironment, NarikUserEnvironment>();
            _unityContainer.RegisterType<IUserPasswordStore<ApplicationUser>, NarikUserStore>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IResourceAccessService, ResourceAccessService>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<ICurrentUserAccessor, NarikCurrentUserAccessor>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IPushService, NarikPushService>(new ContainerControlledLifetimeManager());

        }

        public void RegisterOdataController(ODataModelBuilder builder)
        {

        }



        public void RegisterSignalRHubs(IEndpointRouteBuilder configure)
        {
            configure.MapHub<NarikMessageHub>("/signalr/narikmessages");
        }
    }
}
