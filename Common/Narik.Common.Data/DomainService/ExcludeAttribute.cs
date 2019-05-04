using System;

namespace Narik.Common.Data.DomainService
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ExcludeAttribute : Attribute
    {
    }
}
