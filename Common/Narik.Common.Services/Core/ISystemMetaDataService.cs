using System;
using System.Collections.Generic;
using System.Reflection;
using Narik.Common.Shared.Models;

namespace Narik.Common.Services.Core
{
    public interface  ISystemMetaDataService
    {
        Dictionary<Type, Dictionary<string, NarikApiActionDescriptor>> ApiActions { get; }
        void InitMetaData(Assembly assembly);

        string SystemVersion { get; }

        string DbVersion { get; }
        string EnumVersion { get; }

        string VersionDate { get; }
    }
}
