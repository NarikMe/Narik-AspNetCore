using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Narik.Common.Shared.Models
{
    public class ApplicationUser: IdentityUser<string>
    {

        public int UserId { get; set; }

        public string Title { get; set; }

        public bool IsActive { get; set; }

        public object CustomData { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public override string Id
        {
            get => UserName;
            set => UserName = value;
        }
    }
}
