using System;
using System.Collections.Generic;

namespace Narik.Common.Shared.Models
{
    public class LoginResultModel
    {
        public bool Succeeded { get; set; }
        public Dictionary<string,string> Errors { set; get; }
        public LoginUserInfo LoginedUser { set; get; }

        public object Token { get; set; }
    }

    public class LoginUserInfo
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
