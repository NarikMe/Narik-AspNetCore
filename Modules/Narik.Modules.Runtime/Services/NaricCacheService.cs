using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Narik.Common.Services.Core;

namespace Narik.Modules.Runtime.Services
{
    public class NarikMemoryCacheService: ICacheService
    {

        private readonly ConcurrentDictionary<string,object> _storage =new ConcurrentDictionary<string, object>();
        public void RemoveItems(List<string> keys)
        {
            
        }

        public void RemoveItem(string key)
        {
        }

        public T GetItem<T>(string key, Func<T> callBack)
        {
            return default(T);
        }

        public void Clear()
        {
        }
    }
}
