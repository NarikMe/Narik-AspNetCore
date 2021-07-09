using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Narik.Common.Data;
using Narik.Common.Shared.Interfaces;
using Narik.Common.Shared.Models;


namespace Narik.Common.Web.Infrastructure.OData
{

    public abstract class NarikPagedODataController<T, TViewModel> :
        NarikPagedODataController<T, TViewModel, TViewModel, PostData<TViewModel>, INarikDomainService<INarikDataService>>
        where TViewModel : class, INarikViewModel, new()
        where T : class
    {
    }
    public abstract class NarikPagedODataController<T, TViewModel, TDomainService> :
        NarikPagedODataController<T, TViewModel, TViewModel, PostData<TViewModel>, TDomainService>
        where TViewModel : class, INarikViewModel, new()
        where T : class
        where TDomainService : INarikDomainService<INarikDataService>
    {
    }

    public abstract class NarikPagedODataController<T, TViewModel, TListViewModel, TDomainService> :
        NarikPagedODataController<T, TViewModel, TListViewModel, PostData<TViewModel>, TDomainService>
        where TViewModel : class, INarikViewModel, new()
        where T : class
        where TDomainService : INarikDomainService<INarikDataService>
    {
    }


    public abstract class NarikPagedODataController<T, TViewModel,TListViewModel, TPostData,TDomainService> : 
        NarikODataController<T, TViewModel, TListViewModel, TPostData, TDomainService, long>
         where TViewModel : class, INarikViewModel, new()
        where T : class
        where TDomainService :INarikDomainService<INarikDataService>
        where TPostData : IPostData<TViewModel>
    {

        protected TListViewModel GetEntityFromPagingResult(DbQuery dbQuery, string keyField, long entityId)
        {
            int count;
            return
                GetPagingData(dbQuery, string.Format("{0} = {1}", keyField, entityId), null, 1, 0, out count)
                    .FirstOrDefault();
        }
        protected List<TListViewModel> GetPagingData(DbQuery dbQuery,
            string filter, string sort, int pageSize, int pageIndex,
            out int count)
        {
            if (dbQuery.Parameters == null)
                dbQuery.Parameters = new List<SqlParameter>();
            dbQuery.Parameters.Add(new SqlParameter("PageNumber", pageIndex + 1));
            dbQuery.Parameters.Add(new SqlParameter("RowspPage", pageSize));
            dbQuery.Parameters.Add(new SqlParameter("Sort", string.IsNullOrEmpty(sort) ? "" : sort)
            {
                SqlDbType = SqlDbType.NVarChar
            });
            dbQuery.Parameters.Add(new SqlParameter("Filter", string.IsNullOrEmpty(filter) ? "" : filter)
            {
                SqlDbType = SqlDbType.NVarChar
            });

            dbQuery.Parameters.Insert(0, new SqlParameter { ParameterName = "@Count", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output });
            var paramStr = "";
            for (int i = 1; i < dbQuery.Parameters.Count; i++)
                paramStr += "," + "@" + dbQuery.Parameters[i].ParameterName;
            //TODO:
            //var query = dbQuery.Context.Database.SqlQuery<TListViewModel>(string.Format("Exec @Count={0} {1}", dbQuery.Command, paramStr.Substring(1)), dbQuery.Parameters.ToArray());
           // var data = query.ToList();
           // count = Convert.ToInt32(dbQuery.Parameters[0].Value);
            //return data;
            count = 0;
            return new List<TListViewModel>();
        }

        [EnableQuery]
        public virtual PageResult<TListViewModel> Get(ODataQueryOptions<TListViewModel> queryOptions)
        {
            if (!IsPagingHandledInDb)
            {
                return null;
                //TODO:
                //var data = Query.ApplyODataOption(queryOptions);

                //return new PageResult<TListViewModel>(data, new Uri("http://narik.me"),
                //    Request.ODataProperties().TotalCount);
            }
            else
            {
                var dbQuery = GetDbQuery();
                if (dbQuery != null)
                {
                    if (dbQuery.Parameters == null)
                        dbQuery.Parameters = new List<SqlParameter>();
                    string filter = null, sort = null;
                    int pageSize = 100, pageIndex = 0;

                    if (queryOptions != null)
                    {
                        if (queryOptions.Filter != null)
                            filter = ODataSqlFilterBinder.BindFilterQueryOption(queryOptions.Filter);
                        if (queryOptions.OrderBy != null)
                            sort = queryOptions.OrderBy.RawValue;
                        pageSize = queryOptions.Top.Value;
                        if (queryOptions.Skip != null)
                            pageIndex = (queryOptions.Skip.Value) / pageSize;
                    }

                    int count = 0;
                    var data = GetPagingData(dbQuery, filter, sort, pageSize, pageIndex, out count);
                    return new PageResult<TListViewModel>(data, new Uri("http://narik.me"), count);
                }
                return null;
            }
        }

        

      

        protected virtual DbQuery GetDbQuery()
        {
            return null;
        }

        protected virtual bool IsPagingHandledInDb => false;
    }

    public class DbQuery
    {
        public string Command { get; set; }
        public List<SqlParameter> Parameters { get; set; }

        public DbContext Context { get; set; }
    }
}
