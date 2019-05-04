using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Narik.Common.Services.Core;

namespace Narik.Modules.Runtime.Services
{
    public class NarikAppStororage : IAppStorage
    {
        private readonly ConcurrentDictionary<string, object> _storage = new ConcurrentDictionary<string, object>();
        public object this[string key] => _storage[key];

        public T Get<T>(string key)
        {
            return (T)this[key];
        }
        public void Add(string key, object value)
        {
            _storage.AddOrUpdate(key, value, ((s, o) => value));
        }

        public bool Contains(string key)
        {
            return _storage.ContainsKey(key);
        }
    }
}
