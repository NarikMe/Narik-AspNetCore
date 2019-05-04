using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Narik.Common.Data.DomainService;

namespace Narik.Common.Data
{
    public interface INarikDomainService<out TDataService>
    where TDataService : INarikDataService
    {

        TDataService DataService { get; }

        IQueryable<TListViewModel> GetEntityList<TModel, TListViewModel>(Expression<Func<TModel, bool>> where = null,
            string orderByFiled = null) where TModel : class;
       Task<TViewModel> GetEntityAsync<TModel, TViewModel, TKey>(TKey id, bool useFind = false) where TModel : class;

        Task<bool> Submit(ChangeSet changes, Action<DbContext, ChangeSet> doInStart = null,
            Action<DbContext, ChangeSet> doBeforePersist = null,
            Action<DbContext, ChangeSet> doInEnd = null);
    }
}
