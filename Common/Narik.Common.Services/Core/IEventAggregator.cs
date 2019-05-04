using System;

namespace Narik.Common.Services.Core
{
    public interface  IEventAggregator
    {
        void Register<T>(string key, Action<T> action);
        void Raise(string key, object param) ;
    }
}
