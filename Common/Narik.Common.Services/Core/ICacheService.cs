using System;
using System.Collections.Generic;
using System.Linq;

namespace Narik.Common.Services.Core
{
    public interface ICacheService
    {
        void RemoveItems(List<string> keys);

        void RemoveItem(string key);

        T GetItem<T>(string key,Func<T> callBack);
        void Clear();
    }
}
