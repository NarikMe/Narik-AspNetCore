using System;
using System.Collections.Generic;
using Narik.Common.Services.Core;

namespace Narik.Modules.Runtime.Services
{
    public class NarikEventAggregator : IEventAggregator
    {
        private readonly Dictionary<string, List<Action<object>>> _reporistory =
            new Dictionary<string, List<Action<object>>>();
        public void Register<T>(string key, Action<T> action)
        {
            if (action != null)
            {
                List<Action<object>> items;
                if (_reporistory.ContainsKey(key))
                    items = _reporistory[key];
                else
                {
                    items = new List<Action<object>>();
                    _reporistory.Add(key, items);
                }
                items.Add(o => action((T)o));
            }

        }

        public void Raise(string key, object param)
        {
            if (_reporistory.ContainsKey(key))
            {
                foreach (var action in _reporistory[key])
                {
                    action(param);
                }
            }
        }
    }
}
