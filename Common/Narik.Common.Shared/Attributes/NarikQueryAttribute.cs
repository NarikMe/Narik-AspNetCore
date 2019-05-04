using System;

namespace Narik.Common.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public  sealed class NarikQueryAttribute : Attribute
    {
        public NarikQueryAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public bool IsDbQuery { get; set; }

        public string KeyField { get; set; }
    }
   
  
}
