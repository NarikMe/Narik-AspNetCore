using System;

namespace Narik.Common.Shared.Extensions
{
    public static class TypeExtension
    {
        public static bool IsNumeric(this Type type) { return IsNumeric(type, Type.GetTypeCode(type)); }

        private static bool IsNumeric(Type type, TypeCode typeCode) { return (typeCode == TypeCode.Decimal || (type.IsPrimitive && typeCode != TypeCode.Object && typeCode != TypeCode.Boolean && typeCode != TypeCode.Char)); }
    }
}
