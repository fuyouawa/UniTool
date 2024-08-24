using System.Collections;
using System.Collections.Generic;

namespace UniTool.Utilities
{
    public static class TypeExtension
    {
        public static bool IsNullOrEmpty(this string val)
        {
            return string.IsNullOrEmpty(val);
        }

        public static bool IsNullOrEmpty<T>(this IList<T> val)
        {
            return val == null || val.Count == 0;
        }
    }
}