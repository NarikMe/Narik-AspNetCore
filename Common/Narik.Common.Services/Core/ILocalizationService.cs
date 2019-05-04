using System.Globalization;
using Narik.Common.Shared.Interfaces;

namespace Narik.Common.Services.Core
{
   
    public interface  ILocalizationService
    {
        void AddResource(string key, IObservableResources resource);
        string  GetString(string resourceName, string key);

        string GetString(string resourceName, string key, CultureInfo culture);

        string GetStringWithFormattedKey(string key);

        CultureInfo CurrentUserCulture { get; }
       

    }
}
