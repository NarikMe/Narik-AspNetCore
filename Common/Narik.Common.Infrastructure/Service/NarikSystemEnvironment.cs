using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Narik.Common.Services.Core;

namespace Narik.Common.Infrastructure.Service
{
    public class NarikSystemEnvironment : IEnvironment
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        public NarikSystemEnvironment(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            ModelAssemblies=new List<Assembly>();
        }
        public Assembly MainAssembly { get; set; }
        public IEnumerable<Assembly> ModelAssemblies { get; set; }

        public void AddModelAssembly(Assembly assembly)
        {
            (ModelAssemblies as List<Assembly>)?.Add(assembly);
        }

        public string AppRoot => AppDomain.CurrentDomain.BaseDirectory;
        public string WebRootPath => _hostingEnvironment.WebRootPath;
        public string SaturnResourcesRoot => Path.Combine(AppRoot, "NarikResources");
    }
}
