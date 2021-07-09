using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CommonServiceLocator;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Narik.Common.Data;
using Narik.Common.Data.DomainService;
using Narik.Common.Services.Core;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Narik.Common.Shared.Interfaces;
using Narik.Common.Shared.Models;

namespace Narik.Common.Web.Infrastructure.OData
{
    public enum EntityResultMode
    {
        ByGet,
        ByPostResult
    }
    public interface INarikODataController
    {

    }

    public abstract class NarikPkTypeODataController<T, TViewModel, TKey> :
        NarikODataController<T, TViewModel, TViewModel, PostData<TViewModel>, INarikDomainService<INarikDataService>
            , TKey>
        where TViewModel : class, INarikViewModel<TKey>, new()
        where T : class
    {
    }


    public abstract class NarikPkTypeODataController<T, TViewModel, TKey, TDomainService> :
        NarikODataController<T, TViewModel, TViewModel, PostData<TViewModel>, TDomainService
            , TKey>
        where TViewModel : class, INarikViewModel<TKey>, new()
        where T : class
        where TDomainService : INarikDomainService<INarikDataService>
    {
    }


    public abstract class NarikODataController<T, TViewModel> :
        NarikODataController<T, TViewModel, TViewModel, PostData<TViewModel>, INarikDomainService<INarikDataService>
        , long>
        where TViewModel : class, INarikViewModel<long>, new()
        where T : class
    {
    }



    public abstract class NarikODataController<T, TViewModel, TDomainService> :
        NarikODataController<T, TViewModel, TViewModel, PostData<TViewModel>, TDomainService, long>
        where TViewModel : class, INarikViewModel<long>, new()
        where T : class
        where TDomainService : INarikDomainService<INarikDataService>
    {
    }


    public abstract class NarikODataController<T, TViewModel, TListViewModel, TDomainService> :
        NarikODataController<T, TViewModel, TListViewModel, PostData<TViewModel>, TDomainService, long>
        where TViewModel : class, INarikViewModel<long>, new()
        where T : class
        where TDomainService : INarikDomainService<INarikDataService>
    {
    }



    public abstract class NarikODataController<T, TViewModel, TListViewModel,
        TPostData,
        TDomainService,
        TKey> : ODataController, INarikODataController
        where TViewModel : class, INarikViewModel<TKey>, new()
        where T : class
        where TDomainService : INarikDomainService<INarikDataService>
        where TPostData : IPostData<TViewModel>
    {

        protected virtual TDomainService DomainService => ServiceLocator.Current.GetInstance<TDomainService>();

        protected virtual IQueryable<TListViewModel> Query => DomainService.GetEntityList<T, TListViewModel>();

        protected virtual async Task<TViewModel> GetEntity(TKey id, bool useFind = false)
        {
            return await DomainService.GetEntityAsync<T, TViewModel, TKey>(id, useFind);
        }

        protected virtual TViewModel CreateNewEntity()
        {
            return new TViewModel();
        }

        private void LogModelStateError()
        {
            var value = ModelState.Values.FirstOrDefault();
            if (value?.Errors != null)
            {
                StringBuilder error = new StringBuilder();
                error.Append("Model State Error on:" + GetType());
                error.Append(Environment.NewLine);
                foreach (var error1 in value.Errors)
                {
                    error.Append(error1.Exception?.Message ?? error1.ErrorMessage);
                    error.Append(Environment.NewLine);
                }
                ServiceLocator.Current.GetInstance<ILoggingService>().Log(error.ToString());
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                LogModelStateError();
                return BadRequest(ModelState);
            }
            var changes = new List<ChangeSetEntry>();
            var items = new List<IdVersionCreator<TKey>>((IEnumerable<IdVersionCreator<TKey>>)parameters["items"]);
            changes.AddRange(items.Select(x => new ChangeSetEntry()
            {
                Entity = new TViewModel
                {
                    ViewModelId = x.Id
                },
                Operation = DomainOperation.Delete
            }));


            var error = await DoBeforeDeleteAsync(changes);

            if (!string.IsNullOrEmpty(error))
                return Error(error);

            var dbChanges = MapChangesToDbChanges(null, changes);

            var result = await SubmitChanges(dbChanges);
            if (result is ObjectResult objectResult &&
                ((ServerResponse<TViewModel>)objectResult.Value).IsSucceed)
            {
                await DoAfterDeleteSuccessAsync(changes);
                return Ok(result);
            }
            return BadRequest(result);
        }






        protected virtual async Task<string> DoBeforeDeleteAsync(List<ChangeSetEntry> changes)
        {
            return await Task.FromResult<string>(null);
        }
        protected virtual async Task<bool>  DoAfterDeleteSuccessAsync(List<ChangeSetEntry> changes)
        {
            return await Task.FromResult(true);
        }
        public virtual async Task<IActionResult> Get(TKey key, ODataQueryOptions<TViewModel> queryOptions)
        {
            if (!key.Equals(GetNewEntityKey()))
            {
                try
                {
                    var entity = await GetEntity(key);
                    if (entity != null)
                    {
                        return Ok(CustomizeEntity(entity, entity, EntityResultMode.ByGet));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }


                return NotFound();
            }
            var newEntity = CreateNewEntity();
            newEntity.ViewModelId = GetNewEntityKey();

            //if (typeof(TKey).IsNumeric())
            //    newEntity.ViewModelId = (TKey) Convert.ChangeType(NarikConstant.NewId, typeof(TKey));
            //else
            //    newEntity.ViewModelId = default(TKey);



            return Ok(newEntity);
        }

        protected virtual TKey GetNewEntityKey()
        {
            return default(TKey);
        }

        protected virtual bool IsNewEntity(TViewModel entity)
        {
            return entity.ViewModelId.Equals(GetNewEntityKey());
        }
        protected virtual bool PostIsAlwaysNew()
        {
            return true;
        }
        protected virtual TViewModel CustomizeEntity(TViewModel originalEntity,
            TViewModel entity, EntityResultMode entityResultMode)
        {
            return entity;
        }


        [HttpPost]
        public virtual async Task<ActionResult> Post([FromBody]TPostData postData)
        {
            return await SaveEntity(postData,PostIsAlwaysNew());
        }

        [HttpPut]
        public virtual async Task<ActionResult> Put([FromODataUri] TKey key, [FromBody]TPostData postData)
        {
            return await SaveEntity(postData, false, key);
        }

        public virtual async Task<ActionResult> SaveEntity(TPostData postData, bool? isNew = null, TKey key = default(TKey))
        {
            if (!ModelState.IsValid)
            {
                LogModelStateError();
                return BadRequest(ModelState);
            }
            var entity = postData.Entity;
            if (isNew.HasValue && !isNew.Value)
            {
                if (!entity.ViewModelId.Equals(key))
                    return BadRequest();
            }
            if (!isNew.HasValue)
                isNew = IsNewEntity(entity);

            CompleteBeforeSubmitPost(entity, postData, isNew.Value);

            var error = await ValidateBeforeSubmitPostAsync(entity, postData, isNew.Value);
            if (!string.IsNullOrEmpty(error))
                return Error(error);

            var changes = new List<ChangeSetEntry>
            {
                new ChangeSetEntry(entity,isNew.Value ?
                        DomainOperation.Insert :
                        DomainOperation.Update,
                    true,
                    GetEntityUpdateFiledsInfo(entity))
            };

            AddDependentChanges(entity, postData, changes, isNew.Value);
            ApplyChildChanges(entity, postData, changes, isNew.Value);


            var dbChanges = MapChangesToDbChanges(entity, changes);

            var result = await SubmitChanges(dbChanges,
                DoInStart == null ? (Action<DbContext, ChangeSet>)null : (ex, ch) =>
                    DoInStart(ex, ch, entity),
                DoBeforePersist == null ? (Action<DbContext, ChangeSet>)null : (ex, ch) =>
                    DoBeforePersist(ex, ch, entity),
                DoInEnd == null ? (Action<DbContext, ChangeSet>)null : (ex, ch) =>
                    DoInEnd(ex, ch, entity));
            if (result is ObjectResult objectResult && ((ServerResponse<TViewModel>)objectResult.Value).IsSucceed)
            {
                var resultEntity = CustomizeEntity(entity, ((ServerResponse<TViewModel>)objectResult.Value)?.Data,
                    EntityResultMode.ByPostResult);
                result = Ok(resultEntity);
                DoAfterSubmitSuccess(resultEntity, result);
            }
            return CustomResultAfterPost(result, entity);
        }

        protected virtual void DoAfterSubmitSuccess(TViewModel entity, ActionResult<ServerResponse<TViewModel>> result)
        {

        }
        protected virtual async Task<string> ValidateBeforeSubmitPostAsync(TViewModel entity, TPostData postData, bool isNew)
        {
            return await Task.FromResult<string>(null);
        }

        protected virtual EntityUpdateFiledsInfo GetEntityUpdateFiledsInfo(TViewModel entity)
        {
            return null;
        }

        protected virtual List<ChangeSetEntry> MapChangesToDbChanges(TViewModel entity, List<ChangeSetEntry> changes)
        {
            var mapper = ServiceLocator.Current.GetInstance<IMapper>();
            return mapper.Map<List<ChangeSetEntry>>(changes);
        }

        protected virtual Action<DbContext, ChangeSet, TViewModel> DoInEnd => null;
        protected virtual Action<DbContext, ChangeSet, TViewModel> DoBeforePersist => null;
        protected virtual Action<DbContext, ChangeSet, TViewModel> DoInStart => null;



        protected virtual ActionResult CustomResultAfterPost(ActionResult result, TViewModel entity)
        {
            return result;
        }

        protected virtual void CompleteBeforeSubmitPost(TViewModel entity, TPostData postData, bool isNew)
        {
        }

        protected virtual void AddDependentChanges(TViewModel entity, TPostData postData, List<ChangeSetEntry> changes, bool isNew)
        {
        }

        protected virtual void ApplyChildChanges(TViewModel entity, TPostData postData, List<ChangeSetEntry> changes, bool isNew)
        {
        }



        [HttpPost]
        public virtual async Task<IActionResult> PostMasterDetail(ODataActionParameters parameters)
        {
            return null;
            //return await TaskHelper.StartSaturnTask(() => InternalPostMasterDetail(parameters));
        }

        protected virtual IActionResult InternalPostMasterDetail(ODataActionParameters parameters)
        {
            throw new NotImplementedException();
        }

        protected async Task<ActionResult>
            SubmitChanges(List<ChangeSetEntry> changes,
           Action<DbContext, ChangeSet> doInStart = null,
           Action<DbContext, ChangeSet> doBeforePersist = null,
           Action<DbContext, ChangeSet> doInEnd = null)
        {
            try
            {
                var changeSet = new ChangeSet(changes);
                await DomainService.Submit(changeSet, doInStart, doBeforePersist, doInEnd);
                if (changeSet.HasError)
                    return Ok(new ServerResponse<TViewModel>(false, changeSet.GetErrors()));
                if (changes.Any())
                {
                    var entityViewModel =
                        MapPostResultToViewModel(changes, changes.FirstOrDefault().Entity as T);
                    return EntityContent(entityViewModel);
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                var logService = ServiceLocator.Current.GetInstance<ILoggingService>();
                logService.Log(ex);
                var validationException = ex.InnerException as ValidationException;
                if (validationException != null)
                {
                    ValidationProblem(new ValidationProblemDetails());
                    // Exception(validationException);
                }
                return Exception(ex);
            }
        }

        protected virtual TViewModel MapPostResultToViewModel(List<ChangeSetEntry> changes, T entity)
        {
            var mapper = ServiceLocator.Current.GetInstance<IMapper>();
            return mapper.Map<TViewModel>(entity);
        }


        protected ActionResult Error(string err)
        {
            return new BadRequestObjectResult(new ServerResponse<TViewModel>(false, err));
        }

        protected ActionResult Exception(Exception ex)
        {
            return new BadRequestObjectResult(new ServerResponse<TViewModel>(ex));
        }

        protected ActionResult EntityContent(TViewModel data, bool isSucceed = true)
        {
            return new ObjectResult(new ServerResponse<TViewModel>(isSucceed, data));
        }

        [EnableQuery]
        public virtual IQueryable<TListViewModel> Get(ODataQueryOptions<TListViewModel> queryOptions)
        {
            return Query;
        }
    }

    public class T120<T>
    where T : IComparable
    {
        public T Id { get; set; }
    }

    public class T1201 : T120<decimal>
    {
    }
}
