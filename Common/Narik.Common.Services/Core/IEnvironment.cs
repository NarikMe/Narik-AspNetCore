using System.Collections.Generic;
using System.Reflection;

namespace Narik.Common.Services.Core
{
    public interface IEnvironment
    {

        Assembly MainAssembly { get; set; }

        IEnumerable<Assembly> ModelAssemblies { get; set; }

        void AddModelAssembly(Assembly assembly);

        string AppRoot { get; }
        string WebRootPath { get; }

        string SaturnResourcesRoot { get; }
    }
}
