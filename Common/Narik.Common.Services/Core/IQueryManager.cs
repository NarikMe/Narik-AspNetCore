using System.Collections.Generic;
using System.Reflection;
using Narik.Common.Shared.Attributes;

namespace Narik.Common.Services.Core
{
    public interface  IQueryManager
    {
        void RegisterQueries(string moduleKey,Assembly assembly);

        KeyValuePair<MethodInfo, NarikQueryAttribute> GetQuery(string name);

        MethodInfo GetReviewQuery(string reviewName, string viewName);

       // object GetQueryData(string name,DataSourceRequest dataSourceRequest);

    }
}
