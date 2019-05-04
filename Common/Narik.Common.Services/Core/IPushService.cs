using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Narik.Common.Shared.Models;

namespace Narik.Common.Services.Core
{
    public interface IPushService
    {
        void Send<THub>(string messageKey, object messageData,List<string> groups = null, List<string> users = null)
            where THub : Hub;

        void RegisterConnection(string hubKey, string connectionId, ClientInfo data);

        void RemoveConnection(string hubKey, string connectionId);

        Dictionary<string, ClientInfo> GetConnections(string hubKey);
        void UpdateRegistration(string hubKey, string connectionId, string dbName);
    }
}
