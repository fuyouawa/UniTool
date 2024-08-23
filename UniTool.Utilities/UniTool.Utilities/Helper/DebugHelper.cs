using System;
using UnityEngine;

namespace UniTool.Utilities
{
    public static class DebugHelper
    {
        public static void Assert(bool condition, string message)
        {
            if (UtilitiesConfig.Instance.CheckAssert)
            {
                if (!condition)
                {
                    Debug.unityLogger.Log(LogType.Assert, message);
                }
            }
        }
        public static void Assert(bool condition)
        {
            Assert(condition, "Assertion failed");
        }

        public static void AssertCall(Func<bool> condition, string message)
        {
            if (UtilitiesConfig.Instance.CheckAssert)
            {
                Assert(condition(), message);
            }
        }

        public static void AssertCall(Func<bool> condition)
        {
            Assert(condition());
        }

    }
}