using System.Collections.Generic;
using CommonServiceLocator;
using Microsoft.AspNetCore.SignalR;
using Narik.Common.Services.Core;
using Narik.Common.Shared.Models;

namespace Narik.Modules.Runtime.Services
{
    public class NarikPushService : IPushService
    {
        readonly object _sync = new object();

        protected Dictionary<string, Dictionary<string, ClientInfo>> Connections = new Dictionary<string, Dictionary<string, ClientInfo>>();
        public void Send<THub>(string messageKey, object messageData, List<string> groups = null, List<string> users = null)
            where THub : Hub
        {
            if (groups != null)
                GetHubContext<THub>().Clients.Groups(groups).SendAsync(messageKey, messageData);
            else if (users != null)
                GetHubContext<THub>().Clients.Users(users).SendAsync(messageKey, messageData);
            else
                GetHubContext<THub>().Clients.All.SendAsync(messageKey, messageData);
        }

        public void RegisterConnection(string hubKey, string connectionId, ClientInfo data)
        {
            lock (_sync)
            {
                Dictionary<string, ClientInfo> items;
                if (!Connections.ContainsKey(hubKey))
                {
                    items = new Dictionary<string, ClientInfo>();
                    Connections.Add(hubKey, items);
                }
                else
                    items = Connections[hubKey];
                if (!items.ContainsKey(connectionId))
                    items.Add(connectionId, data);
            }
        }

        public void RemoveConnection(string hubKey, string connectionId)
        {
            lock (_sync)
            {
                if (Connections.ContainsKey(hubKey))
                    Connections[hubKey].Remove(connectionId);
            }

        }

        public Dictionary<string, ClientInfo> GetConnections(string hubKey)
        {
            lock (_sync)
            {
                if (!Connections.ContainsKey(hubKey))
                    return new Dictionary<string, ClientInfo>();
                return Connections[hubKey];
            }
        }

        public void UpdateRegistration(string hubKey,
            string connectionId,
            string dbName)
        {
            lock (_sync)
            {
                if (Connections.ContainsKey(hubKey))
                {
                    var items = Connections[hubKey];
                }
            }

        }

        private IHubContext<THub> GetHubContext<THub>()
            where THub : Hub
        {
            return ServiceLocator.Current.GetInstance<IHubContext<THub>>();
        }
    }
}
