using System;
using System.Diagnostics;

namespace UniTool.Internal
{
    internal static class DebugHelper
    {
        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string message)
        {
            Debug.Assert(condition, message);
        }

        [Conditional("DEBUG")]
        public static void AssertCall(Func<bool> condition)
        {
            Assert(condition());
        }

        [Conditional("DEBUG")]
        public static void AssertCall(Func<bool> condition, string message)
        {
            Assert(condition(), message);
        }
    }
}