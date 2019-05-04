using System;

namespace Narik.Common.Shared.Attributes
{
    public class EngNameAttribute:Attribute
    {
        public EngNameAttribute(string value)
        {
            Value = value;
        }
        public string Value { get; set; }
    }
}
