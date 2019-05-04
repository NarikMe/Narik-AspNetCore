using System;

namespace Narik.Common.Shared.Models
{
    public class ClientInfo
    {
        public string ConnectionId { get; set; }
        public string IP { get; set; }
        public string Browser { get; set; }
        public string userName { get; set; }
        public string UserID { get; set; }
        public string UserTitle { get; set; }
        public string SessionID { get; set; }
        public DateTime ConnectTime { get; set; }

        public Guid ClientId { get; set; }
    }
}
