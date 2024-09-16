using System;
using System.Collections.Generic;
using System.Linq;

namespace UniTool.Utilities
{
    public static class LinqExtension
    {
        public static void ForEach<T>(this IEnumerable<T> enumerator, Action<T> callback)
        {
            foreach (var elem in enumerator)
            {
                callback(elem);
            }
        }

        // public static int IndexOf<T>(this IEnumerable<T> enumerator, T value, int startIndex, int count)
        // {
        //     return Array.IndexOf(enumerator.ToArray(), value, startIndex, count);
        // }
        //
        // public static int IndexOf<T>(this IEnumerable<T> enumerator, T value)
        // {
        //     return Array.IndexOf(enumerator.ToArray(), value);
        // }
        //
        // public static int IndexOf<T>(this IEnumerable<T> enumerator, T value, int startIndex)
        // {
        //     return Array.IndexOf(enumerator.ToArray(), value, startIndex);
        // }
    }
}