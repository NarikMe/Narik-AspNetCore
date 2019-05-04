using System.Collections.Generic;
using System.Reflection;
using Narik.Common.Shared.Interfaces;

namespace Narik.Common.Services.Core
{
    public interface IModuleService
    {

        List<INarikModuleModel> LoadedModules { get; }
        Dictionary<string, Assembly> ModuleAssemblies { get; }


        void InitModules();

        bool IsModuleLoaded(string key);

      
        Assembly GetModuleAssembly(string key);
    }
}
