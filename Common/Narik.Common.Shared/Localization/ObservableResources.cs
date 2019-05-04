using System.Globalization;
using System.Resources;
using Narik.Common.Shared.Interfaces;

namespace Narik.Common.Shared.Localization
{
    public class ObservableResources<T> :  IObservableResources
    {
        private static T _resources;

        public T LR
        {
            get { return _resources; }
        }

        public ResourceManager ResourceManager { get;private  set; }
        
        public string GetString(string key)
        {
            return ResourceManager.GetString(key);
        }
        public string GetString(string key,CultureInfo cultureInfo)
        {
            return ResourceManager.GetString(key, cultureInfo);
        }

        public ObservableResources(T resources)
        {
            _resources = resources;
            ResourceManager=typeof(T).GetProperty("ResourceManager").GetValue(resources, null) as ResourceManager;
        }

        
    }
}
