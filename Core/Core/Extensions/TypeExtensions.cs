using System;

namespace Core.Extensions
{
    internal static class TypeExtensions
    {
        public static bool IsBoolean(this Type type)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            return t == typeof(bool);
        }
    }
}
