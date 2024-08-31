using System;

namespace UniTool.Utilities
{
    public static class ValueExtension
    {
        public static float Round(this float value, int decimals)
        {
            return (float)Math.Round(value, decimals);
        }
        public static float Round(this float value)
        {
            return (float)Math.Round(value);
        }
    }
}