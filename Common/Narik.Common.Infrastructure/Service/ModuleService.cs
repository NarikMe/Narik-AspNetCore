using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Narik.Common.Infrastructure.Interfaces;
using Narik.Common.Services.Core;
using Narik.Common.Shared.Interfaces;
using Narik.Common.Shared.Models;
using Unity;
using Unity.RegistrationByConvention;

namespace Narik.Common.Infrastructure.Service
{
    public class ModuleService : IModuleService
    {
        public const string RuntimeModule = "Runtime";
        
        private readonly IUnityContainer _unityContainer;
        private readonly ILoggingService _loggingService;
        private readonly IEnvironment _environment;
        private IConfiguration _configuration;
        private IOptions<NarikModulesConfig> opts;
        public List<INarikModuleModel> AvaialbleModules { get; private set; }
        private readonly Dictionary<string, Assembly> _moduleAssemlies = new Dictionary<string, Assembly>();
        public ModuleService(IUnityContainer unityContainer, 
            IEnvironment environment, 
            ILoggingService loggingService,
            IConfiguration configuration)
        {
            _unityContainer = unityContainer;
            _loggingService = loggingService;
            _configuration = configuration;
            _environment = environment;

        }


        public Assembly GetModuleAssembly(string key)
        {
            return _moduleAssemlies[key];
        }

        public List<INarikModuleModel> LoadedModules { get; private set; }
        public Dictionary<string, Assembly> ModuleAssemblies => _moduleAssemlies;

        public void InitModules()
        {
            var narikModuleConfig = new NarikModulesConfig();
            _configuration.GetSection("NarikModulesConfig").Bind(narikModuleConfig);

            AvaialbleModules = narikModuleConfig.Modules.OfType<INarikModuleModel>().ToList();

            var runTimeAssemblyPath = Path.Combine(_environment.AppRoot, @"Narik.Modules.Runtime.dll");
            if (!File.Exists(runTimeAssemblyPath))
                throw new Exception($"'Runtime Module'  Not Found in {runTimeAssemblyPath}");
            //Load Runtime
            AssemblyLoadContext.Default
                .LoadFromAssemblyPath(runTimeAssemblyPath);

            if (narikModuleConfig.Modules!=null)
                foreach (var narikModuleModel in narikModuleConfig.Modules)
                {
                   AssemblyLoadContext.Default
                        .LoadFromAssemblyPath(Path.Combine(_environment.AppRoot, narikModuleModel.AssemblyName));
                    if (!string.IsNullOrEmpty(narikModuleModel.Dependencies))
                    {
                        var dependencies =
                            narikModuleModel.Dependencies.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        foreach (var dependency in dependencies)
                        {
                            AssemblyLoadContext.Default
                                .LoadFromAssemblyPath(Path.Combine(_environment.AppRoot, dependency));
                        }
                    }
                }
          

            _unityContainer.RegisterTypes(
             AllClasses.FromLoadedAssemblies(). 
                 Where(type => typeof(INarikModule).IsAssignableFrom(type) && !type.IsInterface)
                 .Distinct(),
             WithMappings.FromAllInterfaces,
             WithName.TypeName,
             WithLifetime.ContainerControlled);


            var modules = _unityContainer.ResolveAll<INarikModule>().ToList();
            _loggingService.Log("Modules Count:"+modules.Count);
          

            foreach (var module in modules)
                _moduleAssemlies.Add(module.Key, module.GetType().Assembly);


            //Init RuntimeModule
            var runtimeModule = modules.FirstOrDefault(x => x.Key == RuntimeModule);
            if (runtimeModule != null)
            {
                InitModule(runtimeModule);
            }
            else
                throw new Exception("Module Runtime Not Found");


            //Init Other Module
            var modulesQuery = from m in modules
                join am in AvaialbleModules
                    on m.Key equals am.Key
                orderby am.InitOrder
                select new { m, am };

            var mustInitModules = modulesQuery.ToList();

            var orderbyQuery = mustInitModules.OrderBy(x => x.am.InitOrder).ToList();

            foreach (var module in orderbyQuery)
            {
                InitModule(module.m);
            }

        }


        private void InitModule(INarikModule module)
        {
            module.Init();
        }
        public bool IsModuleLoaded(string key)
        {
            return LoadedModules.Any(x => x.Key == key);
        }
    }
}
