using System;
using System.Collections.Generic;

namespace Narik.Common.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OdataFunctionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OdataActionParameterInfoAttribute : Attribute
    {

        public string Name { get; set; }
        public Type Type { get; set; }

        public OdataActionParameterInfoAttribute( string name,Type type)
        {
            Name = name;
            Type = type;
        }

    }
}
