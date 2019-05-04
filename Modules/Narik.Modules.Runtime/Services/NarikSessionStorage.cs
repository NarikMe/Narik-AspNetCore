using System;
using System.Collections.Concurrent;
using CommonServiceLocator;
using Microsoft.AspNetCore.Http;
using Narik.Common.Services.Core;

namespace Narik.Modules.Runtime.Services
{
    public class NarikSessionStorage : ISessionStorage
    {
        private readonly object _lock = new object();
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object>> _storage = new ConcurrentDictionary<string, ConcurrentDictionary<string, object>>();
        private readonly ConcurrentDictionary<string, Func<string, object>> _callBacks = new ConcurrentDictionary<string, Func<string, object>>();
        public NarikSessionStorage()
        {
            KeySelector = () =>ServiceLocator.Current.GetInstance<IHttpContextAccessor>()
                .HttpContext?.User?.FindFirst("ClientId")?.Value;
        }


        public void AddCallBack(string key, Func<string, object> callBack)
        {
            _callBacks.TryAdd(key, callBack);
        }
        public object this[string key] => Get<object>(key);

        public T Get<T>(string key)
        {
            var dic = GetDictionary();

            if (!dic.ContainsKey(key))
            {
                lock (_lock)
                {
                    if (_callBacks.ContainsKey(key))
                        dic.TryAdd(key, _callBacks[key](key));
                    else
                        return default(T);
                }
            }
            return (T)dic[key];
        }


        public void Add(string key, object value)
        {
            GetDictionary().AddOrUpdate(key, value, ((s, o) => value));
        }

        public bool Contains(string key)
        {
            return GetDictionary().ContainsKey(key);
        }

        public Func<string> KeySelector { get; set; }


        private ConcurrentDictionary<string, object> GetDictionary()
        {
            return _storage.GetOrAdd(KeySelector(), new ConcurrentDictionary<string, object>());
        }
    }
}
