using System.Reflection;

namespace UniTool.Utilities
{
    public static class BindingFlagsHelper
    {
        public static readonly BindingFlags PublicInstance = BindingFlags.Public |
                                                          BindingFlags.Instance;
        public static readonly BindingFlags PrivateInstance = BindingFlags.NonPublic |
                                                             BindingFlags.Instance;
        public static readonly BindingFlags AllInstance = PublicInstance | PrivateInstance;
    }
}