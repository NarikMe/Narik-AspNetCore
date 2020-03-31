using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using CommonServiceLocator;
using Microsoft.EntityFrameworkCore;
using Narik.Common.Data.DomainService;

namespace Narik.Common.Data
{
    public class BaseDomainService<TDataService>:INarikDomainService<TDataService>
    where TDataService:INarikDataService
    {
        public BaseDomainService()
        {
            DataService = ServiceLocator.Current.GetInstance<TDataService>();
            MapperConfiguration = ServiceLocator.Current.GetInstance<IConfigurationProvider>();
        }

        public TDataService DataService { get; }
        public IConfigurationProvider MapperConfiguration { get; }

        public virtual   IQueryable<TListViewModel> GetEntityList<TModel, TListViewModel>(Expression<Func<TModel, bool>> where = null,
            string orderByFiled = null) where TModel : class
        {
            return  DataService.GetEntityList<TModel, TListViewModel>(where, orderByFiled);
        }

        public virtual async Task<TViewModel> GetEntityAsync<TModel, TViewModel, TKey>(TKey id, bool useFind = false) where TModel : class
        {
            return await DataService.GetEntityAsync<TModel, TViewModel, TKey>(id,useFind);
        }

        public virtual async Task<bool> Submit(ChangeSet changes, Action<DbContext, ChangeSet> doInStart = null, Action<DbContext, ChangeSet> doBeforePersist = null, 
            Action<DbContext, ChangeSet> doInEnd = null)
        {
            return await DataService.Submit(changes, doInStart, doBeforePersist, doInEnd);
        }
    }
}
