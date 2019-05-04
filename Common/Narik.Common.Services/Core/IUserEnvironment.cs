using System;
using System.Collections.Generic;

namespace Narik.Common.Services.Core
{
    public interface IUserEnvironment
    {
        int UserId { get; set; }
        string UserName { get; set; }
        string UserTitle { get; set; }

        string UserCulture { get; set; }

        string UserCultureFullName { get; set; }
        bool CurrentUserIsAdmin { get; }
       
        string DataSource { get;  }
        
        byte? DateType { get; set; }
        string DateFormat { get; set; }

       

        string UserHostAddress { get;  }

        string AppVersion { get; set; }
        string LastVersionDate { get; set; }

        void LoadData(string userName);


        int SessionTimeOutMinute { get; set; }

        string FullCurrentDateString { get; set; }

        string CurrentDateString { get; set; }

        DateTime CurrentDate { get; set; }
        string ConnectionString { get; set; }
        string DbName { get; set; }

        IEnumerable<int> Roles { get; set; }
    }
}
