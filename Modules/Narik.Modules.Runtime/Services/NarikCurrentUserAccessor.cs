using System;
using Microsoft.AspNetCore.Http;
using Narik.Common.Services.Core;

namespace Narik.Modules.Runtime.Services
{
    public class NarikCurrentUserAccessor: ICurrentUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NarikCurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId => Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirst("UserId").Value);
        public string UserName => _httpContextAccessor.HttpContext.User.Identity.Name;
    }
}
