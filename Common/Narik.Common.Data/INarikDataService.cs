using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Narik.Common.Data.DomainService;

namespace Narik.Common.Data
{
    public interface INarikDataService
    {
        Task<bool>  Submit(ChangeSet changes, Action<DbContext, ChangeSet> doInStart = null,
            Action<DbContext, ChangeSet> doBeforePersist = null,
            Action<DbContext, ChangeSet> doInEnd = null);
        Task<TViewModel> GetEntityAsync<TModel, TViewModel, TKey>(TKey id,bool useFind=false) where TModel : class;
        Task<TResult> GetEntityFiledValueAsync<TModel, TResult, TKey>(TKey id,string filedName) where TModel : class;
        IQueryable<TListViewModel> GetEntityList<TModel, TListViewModel>(Expression<Func<TModel, bool>> where = null,
            string orderByFiled=null) where TModel : class;
       
    }
}
