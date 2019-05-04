using System;

namespace Narik.Common.Shared.Attributes
{
    public class NarikApiActionAttribute : Attribute
    {
        public string EngName { get; set; }
        public string Action { get; set; }
        public bool NoCheckAccesslevel { get; set; }
        public bool? CheckWithCreator { get; set; }
        public Type MetadDataType  { get; set; }
    }
}
