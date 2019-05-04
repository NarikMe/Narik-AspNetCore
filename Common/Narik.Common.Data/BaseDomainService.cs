using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CommonServiceLocator;
using Microsoft.EntityFrameworkCore;
using Narik.Common.Data.DomainService;

namespace Narik.Common.Data
{
    public class BaseDomainService<TDataService>:INarikDomainService<TDataService>
    where TDataService:INarikDataService
    {
        private readonly TDataService _dataService;
        public BaseDomainService()
        {
            _dataService = ServiceLocator.Current.GetInstance<TDataService>();
        }

        public TDataService DataService => _dataService;

        public virtual   IQueryable<TListViewModel> GetEntityList<TModel, TListViewModel>(Expression<Func<TModel, bool>> where = null,
            string orderByFiled = null) where TModel : class
        {
            return  _dataService.GetEntityList<TModel, TListViewModel>(where, orderByFiled);
        }

        public virtual async Task<TViewModel> GetEntityAsync<TModel, TViewModel, TKey>(TKey id, bool useFind = false) where TModel : class
        {
            return await _dataService.GetEntityAsync<TModel, TViewModel, TKey>(id,useFind);
        }

        public virtual async Task<bool> Submit(ChangeSet changes, Action<DbContext, ChangeSet> doInStart = null, Action<DbContext, ChangeSet> doBeforePersist = null, 
            Action<DbContext, ChangeSet> doInEnd = null)
        {
            return await _dataService.Submit(changes, doInStart, doBeforePersist, doInEnd);
        }
    }
}
