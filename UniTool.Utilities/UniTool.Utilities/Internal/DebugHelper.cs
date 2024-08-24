using System;
using System.Diagnostics;
using UnityEngine;

namespace UniTool.Utilities
{
    internal static class DebugHelper
    {
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Assert, message);
            }
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            Assert(condition, "Assertion failed");
        }

        [Conditional("DEBUG")]
        public static void AssertCall(Func<bool> condition, string message)
        {
            Assert(condition(), message);
        }

        [Conditional("DEBUG")]
        public static void AssertCall(Func<bool> condition)
        {
            Assert(condition());
        }
    }
}