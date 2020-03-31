using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CommonServiceLocator;
using Narik.Common.Data;
using Narik.Common.Data.DomainService;
using Narik.Common.Shared.Interfaces;

namespace Narik.Common.Web.Infrastructure.OData
{
    public class NarikBaseDataODataController<T, TDomainService> :
        NarikODataController<T, BaseDataViewModel, BaseDataListViewModel, TDomainService>
        where T : class
        where TDomainService : INarikDomainService<INarikDataService>
    {

        protected virtual string OrderByFiled => "ItemOrder";
        
        protected override List<ChangeSetEntry> MapChangesToDbChanges(BaseDataViewModel entity,List<ChangeSetEntry> changes )
        {
            var mapper = ServiceLocator.Current.GetInstance<IMapper>();
            return changes.Select(x => new ChangeSetEntry(
                mapper.Map<T>(x.Entity), x.Operation, x.ReturnEntity, x.EntityUpdateFiledsInfo)).ToList();
        }

        public virtual IQueryable<BaseDataSimpleViewModel> GetForSelector()
        {
            return DomainService.GetEntityList<T, BaseDataSimpleViewModel>(null, OrderByFiled);

        }
    }

    public class BaseDataViewModel : INarikViewModel
    {
        public long ViewModelId
        {
            get { return Id; }
            set { Id = (int)value; }
        }

        public int Id { get; set; } // Id (Primary key)
        public string Title { get; set; } // Title (length: 200)
        public string Description { get; set; } // Description

        public int ItemOrder { get; set; }
    }

    public class BaseDataListViewModel
    {
        public long ViewModelId
        {
            get { return Id; }
            set { Id = (int)value; }
        }

        public int Id { get; set; } // Id (Primary key)
        public string Title { get; set; } // Title (length: 200)

        public int ItemOrder { get; set; }
    }
    public class BaseDataSimpleViewModel
    {

        public int Id { get; set; } // Id (Primary key)
        public string Title { get; set; } // Title (length: 200)

        public int ItemOrder { get; set; }
    }
}


