using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Narik.Common.Data.DomainService
{
    public sealed class ChangeSetEntry
    {
        private object _entity;
        private bool _hasMemberChanges;
        private int _id;
        private DomainOperation _operation;
        private object _originalEntity;
        private object _storeEntity;



        public ChangeSetEntry()
        {
            ReturnEntity = true;
        }

        public ChangeSetEntry(object entity,DomainOperation operation,
            bool returnEntity=true,
            EntityUpdateFiledsInfo entityUpdateFiledsInfo=null):
            this(0,entity,null,operation,returnEntity, 
                entityUpdateFiledsInfo)
        {
        }

        public ChangeSetEntry(int id, object entity, 
            object originalEntity, 
            DomainOperation operation,
            bool returnEntity=true,
            EntityUpdateFiledsInfo entityUpdateFiledsInfo = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            this._id = id;
            this._entity = entity;
            this.OriginalEntity = originalEntity;
            this.Operation = operation;
            ReturnEntity = returnEntity;
            EntityUpdateFiledsInfo = entityUpdateFiledsInfo;
        }


        public EntityUpdateFiledsInfo EntityUpdateFiledsInfo { get; set; }
        public bool ReturnEntity { get; set; }
        public IDictionary<string, int[]> Associations { get; set; }

        
        public IEnumerable<string> ConflictMembers { get; set; }

       // internal System.ServiceModel.DomainServices.Server.DomainOperationEntry DomainOperationEntry { get; set; }

       
        public object Entity
        {
            get
            {
                return this._entity;
            }
            set
            {
                this._entity = value;
            }
        }

        
        public IDictionary<string, object[]> EntityActions { get; set; }

        public bool HasConflict
        {
            get
            {
                return (this.IsDeleteConflict || ((this.ConflictMembers != null) && this.ConflictMembers.Any<string>()));
            }
        }

        public bool HasError
        {
            get
            {
                return (this.HasConflict || ((this.ValidationErrors != null) && this.ValidationErrors.Any<ValidationResultInfo>()));
            }
        }

       
        public bool HasMemberChanges
        {
            get
            {
                return this._hasMemberChanges;
            }
            set
            {
                this._hasMemberChanges = value;
            }
        }

   
        public int Id
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        
        public bool IsDeleteConflict { get; set; }

     
        public DomainOperation Operation
        {
            get
            {
                return this._operation;
            }
            set
            {
                if ((((value != DomainOperation.Query) && (value != DomainOperation.Insert)) && ((value != DomainOperation.Update) && (value != DomainOperation.Delete))) && (value != DomainOperation.None))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "InvalidDomainOperationType", new object[] { Enum.GetName(typeof(DomainOperation), value), "value" }));
                }
                this._operation = value;
            }
        }

        
        public IDictionary<string, int[]> OriginalAssociations { get; set; }

        
        public object OriginalEntity
        {
            get
            {
                return this._originalEntity;
            }
            set
            {
                this._originalEntity = value;
                if (value != null)
                {
                    this._hasMemberChanges = true;
                }
            }
        }

        internal ChangeSetEntry ParentOperation { get; set; }

        
        public object StoreEntity
        {
            get
            {
                return this._storeEntity;
            }
            set
            {
                this._storeEntity = value;
            }
        }

        
        public IEnumerable<ValidationResultInfo> ValidationErrors { get; set; }
    }
}
