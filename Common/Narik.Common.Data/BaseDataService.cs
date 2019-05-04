using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CommonServiceLocator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

using Narik.Common.Data.DomainService;
using Narik.Common.Data.Interfaces;
using Narik.Common.Services.Core;
using Narik.Common.Shared.Extensions;
using Narik.Common.Shared.Models;
using Z.EntityFramework.Plus;

namespace Narik.Common.Data
{
    public abstract class BaseDataService<TContext> : INarikDataService where TContext : DbContext, new()
    {
        private static readonly ConcurrentDictionary<Type, DomainServiceDescription> DomainServiceDescriptions =
            new ConcurrentDictionary<Type, DomainServiceDescription>();

        private static readonly ConcurrentDictionary<Type, string> PrimaryKeys = new ConcurrentDictionary<Type, string>();


        private static readonly MethodInfo _updateMethod;
        private static readonly MethodInfo _deleteMethod;
        private static readonly MethodInfo _insertMethod;

        public DomainServiceDescription DomainServiceDescription { get; set; }
        private ICurrentUserAccessor _userAccessor;

        protected readonly ILoggingService LoggingService;
        private readonly IEntityUpdatePushService _entityUpdatePushService;
        private readonly ICacheService _cacheService;
        protected readonly ILocalizationService LocalizationService;
        private readonly NarikModulesConfig _config;
        public IConfiguration Configuration { get; }
        private readonly object _lock = new object();

        static BaseDataService()
        {
            _updateMethod = typeof(BaseDataService<TContext>).GetMethod("UpdateEntity");
            _insertMethod = typeof(BaseDataService<TContext>).GetMethod("InsertEntity");
            _deleteMethod = typeof(BaseDataService<TContext>).GetMethod("DeleteEntity");
        }

        protected BaseDataService()
        {
            _cacheService = ServiceLocator.Current.GetInstance<ICacheService>();
            _config = ServiceLocator.Current.GetInstance<NarikModulesConfig>();
            LocalizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();
            _entityUpdatePushService = ServiceLocator.Current.GetInstance<IEntityUpdatePushService>();
            LoggingService = ServiceLocator.Current.GetInstance<ILoggingService>();
            
            Configuration = ServiceLocator.Current.GetInstance<IConfiguration>();

            lock (_lock)
            {
                if (!DomainServiceDescriptions.ContainsKey(GetType()))
                    DomainServiceDescriptions.TryAdd(GetType(), new DomainServiceDescription(GetType()));
                DomainServiceDescription = DomainServiceDescriptions[GetType()];
            }

            DbContext = CreateDbContext();
        }



        protected IDbContextTransaction BeginTransaction()
        {
            return DbContext.Database.BeginTransaction();
        }

        protected void CommitTransaction()
        {
            DbContext.Database.CommitTransaction();
        }
        protected virtual DbContextOptions<TContext> GetDbContextOptions()
        {
            var opt = new DbContextOptionsBuilder<TContext>();
            var connectionStringName = _config.ConnectionStringKey ?? "DbContextDB";
            opt.UseSqlServer(Configuration[$"ConnectionString:{connectionStringName}"]);
            return opt.Options;
        }
        protected virtual TContext CreateDbContext()
        {
            var dbContext = (TContext)
                Activator.CreateInstance(typeof(TContext), GetDbContextOptions());// new TContext();

            return dbContext;
        }
        public TContext DbContext { get; set; }
        public void CompleteEntityData(ITrackableModel trackableEntity, EntityState state)
        {
            if (trackableEntity != null)
            {
                _userAccessor = ServiceLocator.Current.GetInstance<ICurrentUserAccessor>();
                if (state == EntityState.Added)
                {
                    trackableEntity.InsertDate = DateTime.Now;
                    trackableEntity.LastUpdateDate = DateTime.Now;
                    trackableEntity.LastEditor = _userAccessor.UserId;
                    trackableEntity.Creator = _userAccessor.UserId;
                }
                else if (state == EntityState.Modified)
                {
                    trackableEntity.InsertDate = DateTime.Now;
                    trackableEntity.LastUpdateDate = DateTime.Now;
                }
            }
        }
        public virtual void InsertEntity<TEntity>(TEntity entity)
          where TEntity : class
        {
            CompleteEntityData(entity as ITrackableModel, EntityState.Added);
            var entityEntry = DbContext.Entry(entity);
            if ((entityEntry.State != EntityState.Added))
                entityEntry.State = EntityState.Added;
            else
                DbContext.Set<TEntity>().Add(entity);
        }

        public virtual void UpdateEntity<TEntity>(TEntity entity,
            EntityUpdateFiledsInfo entityUpdateFiledsInfo)
            where TEntity : class
        {
            CompleteEntityData(entity as ITrackableModel, EntityState.Modified);

            var entityEntry = DbContext.Entry(entity);

            if (entityEntry.State == EntityState.Detached)
                Attach(entity);

            entityEntry.State = EntityState.Modified;

            if (entityUpdateFiledsInfo?.OnlyUpdateFileds != null)
                foreach (var onlyUpdateFiled in entityUpdateFiledsInfo.OnlyUpdateFileds)
                    entityEntry.Property(onlyUpdateFiled).IsModified = true;

            if (entityUpdateFiledsInfo?.PreventFromUpdateFileds != null)
                foreach (var onlyUpdateFiled in entityUpdateFiledsInfo.PreventFromUpdateFileds)
                    entityEntry.Property(onlyUpdateFiled).IsModified = false;
            if (entity is ITrackableModel)
            {
                entityEntry.Property("InsertDate").IsModified = false;
                entityEntry.Property("Creator").IsModified = false;
            }

        }

        protected virtual void Attach<TEntity>(TEntity entity)
            where TEntity : class
        {
            DbContext.Set<TEntity>().Attach(entity);
        }

        public virtual void DeleteEntity<TEntity>(TEntity entity)
            where TEntity : class
        {
            var entityEntry = DbContext.Entry(entity);

            if ((entityEntry.State != EntityState.Deleted))
                entityEntry.State = EntityState.Deleted;
            else
            {
                DbContext.Set<TEntity>().Attach(entity);
                DbContext.Set<TEntity>().Remove(entity);
            }
        }

        protected virtual async Task<bool> PersistChangeSet()
        {
            await DbContext.SaveChangesAsync();
            return true;
        }

        protected virtual bool HanldeException(ChangeSet changeSet, Exception ex)
        {
            //if (ex is DbEntityValidationException)
            //{
            //    var exception = ex as DbEntityValidationException;
            //    var f = from a1 in exception.EntityValidationErrors
            //            from err in a1.ValidationErrors
            //            join a2 in changeSet.ChangeSetEntries
            //                on a1.Entry.Entity equals a2.Entity
            //            select new { a2, err.PropertyName, err.ErrorMessage };
            //    foreach (var entry in f)
            //    {
            //        entry.a2.ValidationErrors = new List<ValidationResultInfo>
            //                            {
            //                                new ValidationResultInfo(entry.ErrorMessage, new[] {entry.PropertyName})
            //                            };
            //    }
            //    return true;
            //}
            if (ex is DbUpdateException)
            {
                var exception = ex as DbUpdateException;
                if (exception.InnerException as DbUpdateException != null)
                {
                    if ((exception.InnerException.InnerException as SqlException) != null)
                    {
                        var sqlExcep = exception.InnerException.InnerException as SqlException;
                        if (sqlExcep.State == 10)
                        {
                            var f = from a1 in exception.Entries
                                    join a2 in changeSet.ChangeSetEntries
                                    on a1.Entity equals a2.Entity
                                    select a2;
                            foreach (var entry in f)
                            {

                                var conflicts = new List<string> { "Version" };
                                entry.ConflictMembers = conflicts;
                                entry.IsDeleteConflict = true;

                            }

                            return true;
                        }
                        if (sqlExcep.State == 12)
                        {
                            var f = from a1 in exception.Entries
                                    join a2 in changeSet.ChangeSetEntries
                                    on a1.Entity equals a2.Entity
                                    select a2;
                            foreach (var entry in f)
                            {
                                entry.ValidationErrors = new List<ValidationResultInfo>
                                {
                                    new ValidationResultInfo(sqlExcep.Message, new[] {""})
                                };
                            }
                            return true;
                        }
                        if (sqlExcep.State == 11 || sqlExcep.Number == 547)
                        {
                            var f = from a1 in exception.Entries
                                    join a2 in changeSet.ChangeSetEntries
                                    on a1.Entity equals a2.Entity
                                    select a2;
                            foreach (var entry in f)
                            {
                                entry.ValidationErrors = new List<ValidationResultInfo>
                                {
                                    new ValidationResultInfo(
                                        "InformationUsed", new[] {"Entity"})
                                };
                            }
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        public virtual async Task<bool> Submit(ChangeSet changeSet, Action<DbContext, ChangeSet> doInStart = null,
            Action<DbContext, ChangeSet> doBeforePersist = null,
            Action<DbContext, ChangeSet> doInEnd = null)
        {
            var logService = ServiceLocator.Current.GetInstance<ILoggingService>();
            bool result;
            try
            {
                result = await InternallSubmit(changeSet, doInStart, doBeforePersist, doInEnd);
            }
            catch (Exception ex)
            {

                logService.Log(ex);
                var handled = HanldeException(changeSet, ex);
                if (!handled)
                    throw;
                return true;
            }

            DbContext.ChangeTracker.DetectChanges();



            if (result)
            {
                var newCacheKeyList = new List<EntityChangeData>();
                var addedInCache = new List<Type>();


                foreach (var changeSetEntry in changeSet.ChangeSetEntries)
                {
                    if (changeSetEntry.Entity is ICachableEntity entity && !addedInCache.Contains(entity.GetType()))
                    {
                        newCacheKeyList.AddRange(entity.CahceKeysList.Select(x=>new EntityChangeData()
                        {
                            DataKey = x.DataKey,
                            ModuleKey = x.ModuleKey
                        }));
                        addedInCache.Add(entity.GetType());
                    }
                }

                foreach (var type in addedInCache)
                {
                    if (type.GetInterfaces().Contains(typeof(IPartialCachableEntity)))
                    {
                        var entity = changeSet.ChangeSetEntries.FirstOrDefault(x => x.Entity.GetType() == type).Entity as IPartialCachableEntity;
                        var changes = entity.GetPartialEntityChangeData(changeSet, null);
                        if (changes != null)
                        {
                            newCacheKeyList.AddRange(changes);
                        }
                    }
                }
                //TODO: distinct
                _cacheService.RemoveItems(newCacheKeyList.Select(x => x.DataKey).Distinct().ToList());
                if (newCacheKeyList.Count > 0)
                    _entityUpdatePushService.NotifyChange(newCacheKeyList, DateTime.Now);

            }
            return result;
        }


        public async Task<TViewModel> GetEntityAsync<TModel, TViewModel, TKey>(TKey id, bool useFind = false) where TModel : class
        {
            if (useFind)
                return Mapper.Map<TModel, TViewModel>(await DbContext.Set<TModel>().FindAsync(id));
            var primaryKey = GetPrimaryKey<TModel>();
            return await DbContext.Set<TModel>().Where(string.Format("{0}={2}{1}{2}", primaryKey, id
                ,typeof(TKey).IsNumeric() ? "":(typeof(TKey)==typeof(char) ? "'":"\"")))
                .ProjectTo<TViewModel>().FirstOrDefaultAsync();
        }

        private string GetPrimaryKey<TModel>() where TModel : class
        {
            return PrimaryKeys.GetOrAdd(typeof(TModel),
                DbContext.Model.FindEntityType(typeof(TModel)).FindPrimaryKey().Properties.Select(x => x.Name).FirstOrDefault());
        }

        public async Task<TResult> GetEntityFiledValueAsync<TModel, TResult, TKey>(TKey id, string filedName) where TModel : class
        {
            if (typeof(TModel).GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(INarikModel<>)))
                return await DbContext.Set<TModel>().Where("Id==" + id).Select(filedName).Cast<TResult>().FirstOrDefaultAsync();
            var entity = await DbContext.Set<TModel>().FindAsync(id);
            return (TResult)typeof(TModel).GetProperty(filedName).GetValue(entity, null);
        }

        public IQueryable<TListViewModel> GetEntityList<TModel, TListViewModel>(Expression<Func<TModel, bool>> where = null,
            string orderByFiled = null) where TModel : class
        {
            IQueryable<TModel> result = DbContext.Set<TModel>();
            if (where != null)
                result = result.Where(where);
            if (!string.IsNullOrEmpty(orderByFiled))
                result = result.OrderBy(orderByFiled);
            return result.ProjectTo<TListViewModel>();
        }


        private async Task<bool> InternallSubmit(ChangeSet changeSet, Action<DbContext, ChangeSet> doInStart = null,
            Action<DbContext, ChangeSet> doBeforePersist = null,
            Action<DbContext, ChangeSet> doInEnd = null)
        {
            var result = true;
            using (var dbContextTransaction = DbContext.Database.BeginTransaction())
            {

                doInStart?.Invoke(DbContext, changeSet);
                if (changeSet != null)
                {
                    MethodInfo method;
                    foreach (var source in changeSet.ChangeSetEntries.Where(x => x.Operation == DomainOperation.Insert))
                    {
                        if (!DomainServiceDescription._insertMethods.TryGetValue(source.Entity.GetType(), out method))
                            method = _insertMethod.MakeGenericMethod(source.Entity.GetType());
                        method.Invoke(this, new[] { source.Entity });
                    }
                    foreach (var source in changeSet.ChangeSetEntries.Where(x => x.Operation == DomainOperation.Delete))
                    {
                        if (!DomainServiceDescription._deleteMethods.TryGetValue(source.Entity.GetType(), out method))
                            method = _deleteMethod.MakeGenericMethod(source.Entity.GetType());
                        method.Invoke(this, new[] { source.Entity });
                    }
                    foreach (var source in changeSet.ChangeSetEntries.Where(x => x.Operation == DomainOperation.Update))
                    {
                        if (!DomainServiceDescription._updateMethods.TryGetValue(source.Entity.GetType(), out method))
                            method = _updateMethod.MakeGenericMethod(source.Entity.GetType());
                        method.Invoke(this, new[] { source.Entity, source.EntityUpdateFiledsInfo });
                    }
                }
                doBeforePersist?.Invoke(DbContext, changeSet);
                result = await PersistChangeSet();
                doInEnd?.Invoke(DbContext, changeSet);
                dbContextTransaction.Commit();
            }
            return result;
        }

        public async Task<int> Update<T>(Expression<Func<T,bool>> selector,
            Expression<Func<T, T>> updateFactory)
        where T : class
        {
            return await DbContext.Set<T>().Where(selector)
                .UpdateAsync(updateFactory);
        }


        protected virtual IQueryable<TEntity> Query<TEntity>(String includes = null)
           where TEntity : Entity, new()
        {
            return Query<TEntity>(false, includes, null);
        }
        protected virtual IQueryable<TEntity> Query<TEntity>(bool withPaging)
          where TEntity : Entity, new()
        {
            return Query<TEntity>(withPaging, null, null);
        }
        protected virtual IQueryable<TEntity> Query<TEntity>(bool withPaging, String includes, IQueryable<TEntity> originalQuery)
 where TEntity : Entity, new()
        {
            IQueryable<TEntity> baseQuery = !string.IsNullOrEmpty(includes) ?
                DbContext.Set<TEntity>().Include(includes).AsNoTracking() : DbContext.Set<TEntity>().AsNoTracking();
            return baseQuery;
        }

        public IQueryable<TEntity> ApplyPaging<TEntity>(IQueryable<TEntity> query)
        {
            return query;
        }

    }
}
