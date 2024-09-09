using System;
using System.Collections.Generic;

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
    }
}