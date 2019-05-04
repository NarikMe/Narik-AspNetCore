using System;
using System.Threading.Tasks;
using CommonServiceLocator;
using Microsoft.AspNetCore.SignalR;
using Narik.Common.Services.Core;

using ClientInfo = Narik.Common.Shared.Models.ClientInfo;


namespace Narik.Common.Infrastructure.SignalR
{
    public abstract class NarikHub: Hub<IPushMessage> 
    {
        private readonly IPushService _pushService;

        protected NarikHub()
        {
            _pushService = ServiceLocator.Current.GetInstance<IPushService>();
        }

        private void RegisterConnection()
        {
            _pushService.RegisterConnection(GetType().Name, Context.ConnectionId,
                new ClientInfo
                {
                    userName = Context.User.Identity.Name,
                    ConnectTime = DateTime.Now,
                    ConnectionId = Context.ConnectionId,
                });
        }
        public override Task OnConnectedAsync()
        {
            RegisterConnection();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _pushService.RemoveConnection(GetType().Name, Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
      
    }
}
