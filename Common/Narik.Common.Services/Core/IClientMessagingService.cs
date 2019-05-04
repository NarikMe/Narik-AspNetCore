using System;
using System.Collections.Generic;
using Narik.Common.Shared.Models;

namespace Narik.Common.Services.Core
{
    public interface  IClientMessagingService
    {
        void UpdateClientInfo(string clientId, string username, string toString, string name);
        void CloseClients(List<Guid> clientIds, string msg);
        void SendMessage(List<Guid> clientIds, string msg);
        IEnumerable<ClientInfo> SubscriberSortedList { get; }
    }
}
