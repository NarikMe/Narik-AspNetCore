using System.Collections.Generic;
using Narik.Common.Data.DomainService;
using Narik.Common.Shared.Models;

namespace Narik.Common.Data.Interfaces
{
    /// <summary>
    /// if entity needs to be cached on client side, it should implement this interface
    /// </summary>
    public interface ICachableEntity
    {
        /// <summary>
        /// List of Cached objects that shoud be updated when a an entity of this type changes(insert,Delete,...)
        /// </summary>
        List<DataInfo> CahceKeysList { get; }
    }

    public interface IPartialCachableEntity
    {
         List<EntityChangeData> GetPartialEntityChangeData(ChangeSet changes, object otherData);
    }
}
