using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Narik.Common.Services.Core;
using Narik.Common.Shared.Interfaces;

namespace Narik.Modules.Runtime.Services
{
    public class NarikLocalizationService: ILocalizationService
    {
        public void AddResource(string key, IObservableResources resource)
        {
            
        }

        public string GetString(string resourceName, string key)
        {
            throw new NotImplementedException();
        }

        public string GetString(string resourceName, string key, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public string GetStringWithFormattedKey(string key)
        {
            throw new NotImplementedException();
        }

        public CultureInfo CurrentUserCulture { get; }
    }
}
