using System.Collections.Generic;
using System.Linq;
using Narik.Common.Shared.Models;

namespace Narik.Common.Data.DomainService
{
    public static class ChangeExtenstion
    {
        public static List<ChangeSetEntry> ApplyToChangeSetList<T>(this Change<T> change,
            List<ChangeSetEntry> changes = null, bool returnEntity = false)
        where T :class
        {
            if (changes == null)
                changes = new List<ChangeSetEntry>();

            if (change.Modified != null)
                changes.AddRange(change.Modified.Select(item => new ChangeSetEntry(item, DomainOperation.Update, returnEntity)));
            if (change.Added != null)
                changes.AddRange(change.Added.Select(item => new ChangeSetEntry(item, DomainOperation.Insert, returnEntity)));
            if (change.Removed != null)
                changes.AddRange(change.Removed.Select(item => new ChangeSetEntry(item, DomainOperation.Delete, returnEntity)));

            return changes;
        }
    }
}
