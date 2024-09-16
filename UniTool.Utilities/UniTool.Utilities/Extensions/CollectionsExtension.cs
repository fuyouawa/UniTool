using System;
using System.Collections.Generic;

namespace UniTool.Utilities
{
    public static class CollectionsExtension
    {
        public static bool IsNullOrEmpty<T>(this IList<T> val)
        {
            return val == null || val.Count == 0;
        }

        public static bool IsNotNullOrEmpty<T>(this IList<T> val)
        {
            return !val.IsNullOrEmpty();
        }
        
        public static int IndexOf<T>(this T[] array, T value, int startIndex, int count)
        {
            return Array.IndexOf(array, value, startIndex, count);
        }
        
        public static int IndexOf<T>(this T[] array, T value)
        {
            return Array.IndexOf(array, value);
        }
        
        public static int IndexOf<T>(this T[] array, T value, int startIndex)
        {
            return Array.IndexOf(array, value, startIndex);
        }

        public static void AddRange<T>(this IList<T> val, IEnumerable<T> enumerable)
        {
            foreach (var e in enumerable)
            {
                val.Add(e);
            }
        }
    }
}