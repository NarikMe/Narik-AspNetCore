using System;
using System.Collections.Generic;

namespace Narik.Common.Shared.Interfaces
{
    public interface IWebApiActionMetaData
    {
        Func<Dictionary<string, object>, int[]> FuncGetCreators { get;  }
        Func<Dictionary<string, object>, bool> FuncCheckWithCreator { get;  }
        Func<Dictionary<string, object>, string> FuncAction { get;  }

        Func<Dictionary<string, object>, string> FuncEngName { get; }
    }

   
}
