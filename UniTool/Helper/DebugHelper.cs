using System;
using UniTool.Config;
using UnityEngine;

namespace UniTool.Helper
{
    public static class DebugHelper
    {
        public static void Assert(bool condition, string message)
        {
            if (UniToolConfig.CheckAssert)
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
            if (UniToolConfig.CheckAssert)
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