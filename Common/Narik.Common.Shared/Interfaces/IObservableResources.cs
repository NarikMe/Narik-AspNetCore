using System.Globalization;
using System.Resources;

namespace Narik.Common.Shared.Interfaces
{
    public interface IObservableResources
    {
        ResourceManager ResourceManager { get; }
        string GetString(string key);
        string GetString(string key, CultureInfo cultureInfo);
    }
}
