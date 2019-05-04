using System;
using System.Collections.Generic;
using System.Text;

namespace Narik.Common.Services.Core
{
    public interface IAppStorage
    {
        object this[string key] { get; }

        T Get<T>(string key);
        void Add(string key, object value);
        bool Contains(string key);
    }
}
