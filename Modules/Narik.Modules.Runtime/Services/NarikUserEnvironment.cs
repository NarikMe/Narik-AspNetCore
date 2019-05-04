using System;
using System.Collections.Generic;
using Narik.Common.Services.Core;

namespace Narik.Modules.Runtime.Services
{
    public class NarikUserEnvironment: IUserEnvironment
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserTitle { get; set; }
        public string UserCulture { get; set; }
        public string UserCultureFullName { get; set; }
        public bool CurrentUserIsAdmin { get; }
        public string DataSource { get; }
        public byte? DateType { get; set; }
        public string DateFormat { get; set; }
        public string UserHostAddress { get; }
        public string AppVersion { get; set; }
        public string LastVersionDate { get; set; }
        public void LoadData(string userName)
        {
            
        }

        public int SessionTimeOutMinute { get; set; }
        public string FullCurrentDateString { get; set; }
        public string CurrentDateString { get; set; }
        public DateTime CurrentDate { get; set; }
        public string ConnectionString { get; set; }
        public string DbName { get; set; }
        public IEnumerable<int> Roles { get; set; }
    }
}
