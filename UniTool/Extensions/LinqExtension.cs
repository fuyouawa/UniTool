using System;
using System.Collections;
using System.Collections.Generic;

namespace UniTool.Extension
{
    public static class LinqExtension
    {
        public static void Foreach<T>(this IEnumerable<T> enumerator, Action<T> callback)
        {
            foreach (var elem in enumerator)
            {
                callback(elem);
            }
        }
    }
}