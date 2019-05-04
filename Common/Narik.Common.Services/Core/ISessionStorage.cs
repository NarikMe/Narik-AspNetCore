using System;

namespace Narik.Common.Services.Core
{
    public interface ISessionStorage
    {
        object this[string key] { get; }
        T Get<T>(string key);
        void Add(string key, object value);
        bool Contains(string key);

        Func<string> KeySelector { set; }

        void AddCallBack(string key, Func<string, object> callBack);
    }
}
