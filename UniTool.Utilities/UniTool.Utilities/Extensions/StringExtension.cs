using System.Collections;
using System.Collections.Generic;

namespace UniTool.Utilities
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string val)
        {
            return string.IsNullOrEmpty(val);
        }
        public static bool IsNotNullOrEmpty(this string val)
        {
            return !string.IsNullOrEmpty(val);
        }
    }
}