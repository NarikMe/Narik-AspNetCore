using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Narik.Common.Data.DomainService
{
    public sealed class ChangeSet
    {
        private Dictionary<object, Dictionary<PropertyDescriptor, IEnumerable<ChangeSetEntry>>> _associatedChangesMap;
        private Dictionary<object, List<AssociatedEntityInfo>> _associatedStoreEntities;
        private IEnumerable<ChangeSetEntry> _changeSetEntries;
        private Dictionary<object, object> _entitiesToReplace;
        private Dictionary<object, ChangeOperation> _entityStatusMap;


        public  string[] GetErrors()
        {
            var errors = from source in _changeSetEntries.Where(i => i.HasError && i.ValidationErrors != null)
                from validationResultInfo in source.ValidationErrors
                select validationResultInfo.Message;

            var errors2 = from source in _changeSetEntries.Where(i => i.HasError && i.ConflictMembers != null)
                from conflict in source.ConflictMembers
                select "Version Error";
            

            return  errors.Union(errors2).ToArray();
        }

       
        public ChangeSet(ChangeSetEntry changeSetEntry) :this(new List<ChangeSetEntry> {changeSetEntry})
        {
            
        }
        public ChangeSet(IEnumerable<ChangeSetEntry> changeSetEntries)
        {
            if (changeSetEntries == null)
            {
                throw new ArgumentNullException("changeSetEntries");
            }
            //ValidateChangeSetEntries(changeSetEntries);
            bool flag = false;
            this._entityStatusMap = new Dictionary<object, ChangeOperation>();
            foreach (ChangeSetEntry entry in changeSetEntries)
            {
                object entity = entry.Entity;
                switch (entry.Operation)
                {
                    case DomainOperation.None:
                        this._entityStatusMap[entity] = ChangeOperation.None;
                        break;

                    case DomainOperation.Insert:
                        this._entityStatusMap[entity] = ChangeOperation.Insert;
                        break;

                    case DomainOperation.Update:
                        this._entityStatusMap[entity] = ChangeOperation.Update;
                        break;

                    case DomainOperation.Delete:
                        this._entityStatusMap[entity] = ChangeOperation.Delete;
                        break;
                }
                
            }
            this._changeSetEntries = changeSetEntries;
            if (flag)
            {
                this._changeSetEntries = this.OrderChangeset(changeSetEntries);
            }
        }

        internal void ApplyAssociatedStoreEntityTransforms()
        {
            foreach (KeyValuePair<object, List<AssociatedEntityInfo>> pair in this.AssociatedStoreEntities)
            {
                pair.Value.ForEach(m => m.ApplyTransform());
            }
        }

        public void Associate<TEntity, TStoreEntity>(TEntity clientEntity, TStoreEntity storeEntity, Action<TEntity, TStoreEntity> storeToClientTransform) where TEntity : class where TStoreEntity : class
        {
            if (clientEntity == null)
            {
                throw new ArgumentNullException("clientEntity");
            }
            if (storeEntity == null)
            {
                throw new ArgumentNullException("storeEntity");
            }
            if (storeToClientTransform == null)
            {
                throw new ArgumentNullException("storeToClientTransform");
            }
            this.VerifyExistsInChangeset(clientEntity);
            Action entityTransform = () => storeToClientTransform(clientEntity, storeEntity);
            AssociatedEntityInfo item = new AssociatedEntityInfo(clientEntity, entityTransform);
            List<AssociatedEntityInfo> list = null;
            if (!this.AssociatedStoreEntities.TryGetValue(storeEntity, out list))
            {
                list = new List<AssociatedEntityInfo>();
                this.AssociatedStoreEntities[storeEntity] = list;
            }
            list.Add(item);
        }

        internal void CommitReplacedEntities()
        {
            if (this.EntitiesToReplace.Count > 0)
            {
                foreach (ChangeSetEntry entry in from eo in this.ChangeSetEntries
                                                 where !eo.HasError
                                                 select eo)
                {
                    object obj2;
                    if (this.EntitiesToReplace.TryGetValue(entry.Entity, out obj2))
                    {
                        entry.Entity = obj2;
                    }
                }
            }
        }

        private Dictionary<PropertyDescriptor, IEnumerable<ChangeSetEntry>> GetAssociatedChanges(object entity)
        {
            Func<ChangeSetEntry, bool> predicate = null;
            Func<int, ChangeSetEntry> selector = null;
            Func<int, ChangeSetEntry> func3 = null;
            Dictionary<PropertyDescriptor, IEnumerable<ChangeSetEntry>> dictionary = null;
            if (this._associatedChangesMap == null)
            {
                this._associatedChangesMap = new Dictionary<object, Dictionary<PropertyDescriptor, IEnumerable<ChangeSetEntry>>>();
            }
            else if (this._associatedChangesMap.TryGetValue(entity, out dictionary))
            {
                return dictionary;
            }
            Dictionary<int, ChangeSetEntry> entityOperationMap = this._changeSetEntries.ToDictionary<ChangeSetEntry, int>(p => p.Id);
            dictionary = new Dictionary<PropertyDescriptor, IEnumerable<ChangeSetEntry>>();
            foreach (PropertyDescriptor descriptor in from p in TypeDescriptor.GetProperties(entity.GetType()).Cast<PropertyDescriptor>()
                                                      where p.Attributes[typeof(CompositionAttribute)] != null
                                                      select p)
            {
                List<ChangeSetEntry> list = new List<ChangeSetEntry>();
                if (predicate == null)
                {
                    predicate = p => p.Entity == entity;
                }
                ChangeSetEntry entry = this._changeSetEntries.Single<ChangeSetEntry>(predicate);
                if ((entry.Associations != null) && entry.Associations.ContainsKey(descriptor.Name))
                {
                    int[] source = entry.Associations[descriptor.Name];
                    if (selector == null)
                    {
                        selector = p => entityOperationMap[p];
                    }
                    IEnumerable<ChangeSetEntry> collection = source.Select<int, ChangeSetEntry>(selector);
                    list.AddRange(collection);
                }
                if ((entry.OriginalAssociations != null) && entry.OriginalAssociations.ContainsKey(descriptor.Name))
                {
                    int[] numArray2 = entry.OriginalAssociations[descriptor.Name];
                    if (func3 == null)
                    {
                        func3 = p => entityOperationMap[p];
                    }
                    IEnumerable<ChangeSetEntry> enumerable3 = from p in numArray2.Select<int, ChangeSetEntry>(func3)
                                                              where p.Operation == DomainOperation.Delete
                                                              select p;
                    list.AddRange(enumerable3);
                }
                dictionary[descriptor] = list;
            }
            this._associatedChangesMap[entity] = dictionary;
            return dictionary;
        }

        public IEnumerable GetAssociatedChanges<TEntity, TResult>(TEntity entity, Expression<Func<TEntity, TResult>> expression)
        {
            return this.GetAssociatedChangesInternal(entity, expression, null);
        }

        public IEnumerable GetAssociatedChanges<TEntity, TResult>(TEntity entity, Expression<Func<TEntity, TResult>> expression, ChangeOperation operationType)
        {
            return this.GetAssociatedChangesInternal(entity, expression, new ChangeOperation?(operationType));
        }

        private IEnumerable GetAssociatedChangesInternal(object entity, LambdaExpression expression, ChangeOperation? operationType)
        {
            Func<ChangeSetEntry, bool> predicate = null;
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            this.VerifyExistsInChangeset(entity);
            MemberInfo member = null;
            MemberExpression body = expression.Body as MemberExpression;
            if (body != null)
            {
                member = body.Member;
            }
            if (member == null)
            {
                throw new ArgumentException("ChangeSet_InvalidMemberExpression", "expression");
            }
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(entity.GetType())[member.Name];
            if (descriptor.Attributes[typeof(AssociationAttribute)] == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "MemberNotAnAssociation", new object[] { member.DeclaringType, member.Name }), "expression");
            }
            if (descriptor.Attributes[typeof(CompositionAttribute)] == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "MemberNotAComposition", new object[] { member.DeclaringType, member.Name }), "expression");
            }
            IEnumerable<ChangeSetEntry> source = this.GetAssociatedChanges(entity)[descriptor];
            if (!operationType.HasValue)
            {
                return (from p in source select p.Entity);
            }
            if (predicate == null)
            {
                predicate = p => this.GetChangeOperation(p.Entity) == ((ChangeOperation)operationType);
            }
            return (from p in source.Where<ChangeSetEntry>(predicate) select p.Entity);
        }

        public IEnumerable<TEntity> GetAssociatedEntities<TEntity, TStoreEntity>(TStoreEntity storeEntity) where TEntity : class
        {
            Func<AssociatedEntityInfo, object> selector = null;
            if (storeEntity == null)
            {
                throw new ArgumentNullException("storeEntity");
            }
            List<AssociatedEntityInfo> list = null;
            if (!this.AssociatedStoreEntities.TryGetValue(storeEntity, out list))
            {
                return Enumerable.Empty<TEntity>();
            }
            if (selector == null)
            {
                selector = ai => ai.ClientEntity;
            }
            return list.Select<AssociatedEntityInfo, object>(selector).OfType<TEntity>();
        }

        public ChangeOperation GetChangeOperation(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            ChangeOperation none = ChangeOperation.None;
            this._entityStatusMap.TryGetValue(entity, out none);
            return none;
        }

        public TEntity GetOriginal<TEntity>(TEntity clientEntity) where TEntity : class
        {
            if (clientEntity == null)
            {
                throw new ArgumentNullException("clientEntity");
            }
            ChangeSetEntry entry = this._changeSetEntries.FirstOrDefault<ChangeSetEntry>(p => object.ReferenceEquals(p.Entity, clientEntity));
            if (entry == null)
            {
                throw new ArgumentException("ChangeSet_ChangeSetEntryNotFound");
            }
            if (entry.Operation == DomainOperation.Insert)
            {
                throw new InvalidOperationException("ChangeSet_OriginalNotValidForInsert");
            }
            return (TEntity)entry.OriginalEntity;
        }

        internal bool HasChildChanges(object entity)
        {
            return (this.GetAssociatedChanges(entity).Count > 0);
        }

        private IEnumerable<ChangeSetEntry> OrderChangeset(IEnumerable<ChangeSetEntry> changeSetEntries)
        {
            Dictionary<int, ChangeSetEntry> dictionary = changeSetEntries.ToDictionary<ChangeSetEntry, int>(p => p.Id);
            Dictionary<ChangeSetEntry, List<ChangeSetEntry>> operationChildMap = new Dictionary<ChangeSetEntry, List<ChangeSetEntry>>();
            bool flag = false;
            foreach (IGrouping<Type, ChangeSetEntry> grouping in from p in changeSetEntries group p by p.Entity.GetType())
            {
                IEnumerable<PropertyDescriptor> enumerable = (from p in TypeDescriptor.GetProperties(grouping.Key).Cast<PropertyDescriptor>()
                                                              where p.Attributes[typeof(CompositionAttribute)] != null
                                                              select p).ToArray<PropertyDescriptor>();
                foreach (ChangeSetEntry entry in grouping)
                {
                    foreach (PropertyDescriptor descriptor in enumerable)
                    {
                        flag = true;
                        List<int> source = new List<int>();
                        if ((entry.Associations != null) && entry.Associations.ContainsKey(descriptor.Name))
                        {
                            source.AddRange(entry.Associations[descriptor.Name]);
                        }
                        if ((entry.OriginalAssociations != null) && entry.OriginalAssociations.ContainsKey(descriptor.Name))
                        {
                            source.AddRange(entry.OriginalAssociations[descriptor.Name]);
                        }
                        foreach (int num in source.Distinct<int>())
                        {
                            ChangeSetEntry entry2 = null;
                            if (dictionary.TryGetValue(num, out entry2))
                            {
                                List<ChangeSetEntry> list2;
                                if (entry2.ParentOperation != null)
                                {
                                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "ChangeSet_ChildHasMultipleParents", new object[] { entry2.Id }));
                                }
                                entry2.ParentOperation = entry;
                                if (!operationChildMap.TryGetValue(entry, out list2))
                                {
                                    list2 = new List<ChangeSetEntry>();
                                    operationChildMap[entry] = list2;
                                }
                                list2.Add(entry2);
                            }
                        }
                    }
                }
            }
            if (!flag)
            {
                return changeSetEntries;
            }
            List<ChangeSetEntry> orderedOperations = new List<ChangeSetEntry>();
            foreach (ChangeSetEntry entry3 in from p in changeSetEntries
                                              where p.ParentOperation == null
                                              select p)
            {
                this.OrderOperations(entry3, operationChildMap, orderedOperations);
            }
            return orderedOperations.Union<ChangeSetEntry>(changeSetEntries).ToArray<ChangeSetEntry>();
        }

        private void OrderOperations(ChangeSetEntry operation, Dictionary<ChangeSetEntry, List<ChangeSetEntry>> operationChildMap, List<ChangeSetEntry> orderedOperations)
        {
            List<ChangeSetEntry> list;
            orderedOperations.Add(operation);
            if (operationChildMap.TryGetValue(operation, out list))
            {
                foreach (ChangeSetEntry entry in list)
                {
                    this.OrderOperations(entry, operationChildMap, orderedOperations);
                }
            }
        }

        public void Replace<TEntity>(TEntity clientEntity, TEntity returnedEntity) where TEntity : class
        {
            if (clientEntity == null)
            {
                throw new ArgumentNullException("clientEntity");
            }
            if (returnedEntity == null)
            {
                throw new ArgumentNullException("returnedEntity");
            }
            Type type = clientEntity.GetType();
            Type type2 = returnedEntity.GetType();
            if (type != type2)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "ChangeSet_Replace_EntityTypesNotSame", new object[] { type, returnedEntity }));
            }
            this.VerifyExistsInChangeset(clientEntity);
            this.EntitiesToReplace[clientEntity] = returnedEntity;
        }

        private static void ValidateAssociationMap(Type entityType, HashSet<int> idSet, IDictionary<string, int[]> associationMap)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
            foreach (KeyValuePair<string, int[]> pair in associationMap)
            {
                string key = pair.Key;
                PropertyDescriptor descriptor = properties[key];
                if ((descriptor == null) || (descriptor.Attributes[typeof(AssociationAttribute)] == null))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "InvalidChangeSet", new object[] { string.Format(CultureInfo.CurrentCulture, "InvalidChangeSet_InvalidAssociationMember", new object[] { entityType, key }) }));
                }
                if (pair.Value == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "InvalidChangeSet", new object[] { string.Format(CultureInfo.CurrentCulture, "InvalidChangeSet_AssociatedIdsCannotBeNull", new object[] { entityType, key }) }));
                }
                foreach (int num in pair.Value)
                {
                    if (!idSet.Contains(num))
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "InvalidChangeSet", new object[] { string.Format(CultureInfo.CurrentCulture, "InvalidChangeSet_AssociatedIdNotInChangeset", new object[] { num, entityType, key }) }));
                    }
                }
            }
        }

        private static void ValidateChangeSetEntries(IEnumerable<ChangeSetEntry> changeSetEntries)
        {
            HashSet<int> idSet = new HashSet<int>();
            HashSet<object> set2 = new HashSet<object>();
            foreach (ChangeSetEntry entry in changeSetEntries)
            {
                if (entry.Entity == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "InvalidChangeSet", new object[] { "InvalidChangeSet_NullEntity" }));
                }
                if (idSet.Contains(entry.Id))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "InvalidChangeSet", new object[] { "InvalidChangeSet_DuplicateId" }));
                }
                idSet.Add(entry.Id);
                if (set2.Contains(entry.Entity))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "InvalidChangeSet", new object[] { "InvalidChangeSet_DuplicateEntity" }));
                }
                set2.Add(entry.Entity);
                if ((entry.OriginalEntity != null) && (entry.Entity.GetType() != entry.OriginalEntity.GetType()))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "InvalidChangeSet", new object[] { "InvalidChangeSet_MustBeSameType" }));
                }
                if ((entry.Operation == DomainOperation.Insert) && (entry.OriginalEntity != null))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "InvalidChangeSet", new object[] { "InvalidChangeSet_InsertsCantHaveOriginal" }));
                }
            }
            foreach (ChangeSetEntry entry2 in changeSetEntries)
            {
                if (entry2.Associations != null)
                {
                    ValidateAssociationMap(entry2.Entity.GetType(), idSet, entry2.Associations);
                }
                if (entry2.OriginalAssociations != null)
                {
                    ValidateAssociationMap(entry2.Entity.GetType(), idSet, entry2.OriginalAssociations);
                }
            }
        }

        private void VerifyExistsInChangeset(object entity)
        {
            if (this._changeSetEntries.FirstOrDefault<ChangeSetEntry>(p => object.ReferenceEquals(entity, p.Entity)) == null)
            {
                throw new ArgumentException("ChangeSet_ChangeSetEntryNotFound", "entity");
            }
        }

        private Dictionary<object, List<AssociatedEntityInfo>> AssociatedStoreEntities
        {
            get
            {
                if (this._associatedStoreEntities == null)
                {
                    this._associatedStoreEntities = new Dictionary<object, List<AssociatedEntityInfo>>();
                }
                return this._associatedStoreEntities;
            }
        }

        public IEnumerable<ChangeSetEntry> ChangeSetEntries
        {
            get { return this._changeSetEntries; }
        }

        internal Dictionary<object, object> EntitiesToReplace
        {
            get
            {
                if (this._entitiesToReplace == null)
                {
                    this._entitiesToReplace = new Dictionary<object, object>();
                }
                return this._entitiesToReplace;
            }
        }

        public bool HasError
        {
            get
            {
                return this._changeSetEntries.Any<ChangeSetEntry>(op => (op.HasConflict || ((op.ValidationErrors != null) && op.ValidationErrors.Any<ValidationResultInfo>())));
            }
        }

        private class AssociatedEntityInfo
        {
            private object _clientEntity;
            private Action _entityTransform;

            public AssociatedEntityInfo(object clientEntity, Action entityTransform)
            {
                if (clientEntity == null)
                {
                    throw new ArgumentNullException("clientEntity");
                }
                if (entityTransform == null)
                {
                    throw new ArgumentNullException("entityTransform");
                }
                this._clientEntity = clientEntity;
                this._entityTransform = entityTransform;
            }

            public void ApplyTransform()
            {
                try
                {
                    this._entityTransform();
                }
                catch (TargetInvocationException exception)
                {
                    if (exception.InnerException != null)
                    {
                        throw exception.InnerException;
                    }
                    throw;
                }
            }

            public object ClientEntity
            {
                get
                {
                    return this._clientEntity;
                }
            }
        }
    }
}
