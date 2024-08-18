using System;

namespace UniTool.Extension
{
    public static class TypeExtension
    {
        public static bool IsInherit<T>(this Type t)
        {
            return typeof(T).IsAssignableFrom(t);
        }
    }
}