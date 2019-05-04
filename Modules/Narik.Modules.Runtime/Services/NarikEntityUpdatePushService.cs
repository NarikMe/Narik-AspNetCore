using System;
using System.Collections.Generic;
using System.Text;
using Narik.Common.Services.Core;
using Narik.Common.Shared.Models;
using Narik.Modules.Runtime.Hubs;

namespace Narik.Modules.Runtime.Services
{
    public class NarikEntityUpdatePushService: IEntityUpdatePushService
    {
        private readonly IPushService _pushService;
        private const string MessageKey = "DATA-CHANGE";
        public NarikEntityUpdatePushService(IPushService pushService)
        {
            _pushService = pushService;
        }

        public void NotifyChange(List<EntityChangeData> newCacheKeyList, DateTime now)
        {
            _pushService.Send<NarikMessageHub>(MessageKey,newCacheKeyList);
        }

        public void NotifyChange(List<string> newCacheKeyList, DateTime now)
        {
            throw new NotImplementedException();
        }

        public void ResetCacheKeys(DateTime now)
        {
            throw new NotImplementedException();
        }

        public void SendInitData(string dbName)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, Dictionary<string, DateTime>> GetCahcedItems()
        {
            throw new NotImplementedException();
        }
    }
}
