using System;
using System.Collections.Generic;
using Narik.Common.Shared.Models;

namespace Narik.Common.Services.Core
{
    public interface  IEntityUpdatePushService
    {
        void NotifyChange(List<EntityChangeData> newCacheKeyList, DateTime now);

        void NotifyChange(List<string> newCacheKeyList, DateTime now);
        void ResetCacheKeys(DateTime now);

        void SendInitData(string dbName);

        void Init();

        Dictionary<string, Dictionary<string, DateTime>> GetCahcedItems();
        
    }
}
