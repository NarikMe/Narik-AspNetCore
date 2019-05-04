
using CommonServiceLocator;
using Narik.Common.Infrastructure.Service;
using Narik.Common.Services.Core;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.ServiceLocation;


namespace Narik.Common.Infrastructure.Startup
{
    public static class UnityConfig
    {
        public static IUnityContainer RegisterComponents(IUnityContainer container)
        {

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            container.RegisterType<ILoggingService, NarikLoggingService>
            (new ContainerControlledLifetimeManager(), new InjectionConstructor(DefaultLoggers.ServerLogging));
            container.RegisterType<ILoggingService, NarikLoggingService>
                (DefaultLoggers.ServerLogging, new ContainerControlledLifetimeManager(), new InjectionConstructor(DefaultLoggers.ServerLogging));
            container.RegisterType<ILoggingService, NarikLoggingService>
            (DefaultLoggers.ClinetLogging, new ContainerControlledLifetimeManager(), new InjectionConstructor(DefaultLoggers.ClinetLogging));


            container.RegisterType<IModuleService, ModuleService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEnvironment, NarikSystemEnvironment>(new ContainerControlledLifetimeManager());
           
            return container;
        }
    }
}