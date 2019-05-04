using System.Collections.Generic;
using Narik.Common.Shared.Models;

namespace Narik.Common.Services.Core
{
    public interface IConfigService
    {
        string this[string configKey] { get; }
        bool Contains(string configKey);
        bool Contains(List<string> configKeys);
        void AddConfiguration(Dictionary<string, string> configItems);

        List<ConfigData> GetConfigData(bool includeValue);
        Dictionary<string, string> Configs { get; }
        void Init();
    }
}
