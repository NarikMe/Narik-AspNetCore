using System;

namespace Narik.Common.Shared.Attributes
{
    public class MasterDetailControllerAttribute:Attribute
    {
        public MasterDetailControllerAttribute(Type postMetaDataType)
        {
            ControllerMetaDataType = postMetaDataType;
        }

        public Type ControllerMetaDataType { get; set; }
    }
}
