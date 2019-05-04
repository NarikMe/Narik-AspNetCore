

namespace Narik.Common.Shared.Models
{
    public class EntityChangeData
    {
        public string DataKey { get; set; }
        public string ModuleKey { get; set; }
    }
    public class PartialEntityChangeData : EntityChangeData
    {
        public PartialEntityChangeData()
        {
            Updated = new long[] { };
            Deleted = new long[] { };
        }
        public long[] Updated { get; set; }
        public long[] Deleted { get; set; }
    }
}
