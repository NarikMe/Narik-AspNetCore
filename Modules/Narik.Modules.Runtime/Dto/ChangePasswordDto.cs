using System;
using System.Collections.Generic;
using System.Text;

namespace Narik.Modules.Runtime.Dto
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string Password { get; set; }
    }
}
