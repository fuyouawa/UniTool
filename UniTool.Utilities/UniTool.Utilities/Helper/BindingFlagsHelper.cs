using System.Reflection;

namespace UniTool.Utilities
{
    public static class BindingFlagsHelper
    {
        public static readonly BindingFlags PublicInstance = BindingFlags.Public |
                                                          BindingFlags.Instance;
        public static readonly BindingFlags NoPublicInstance = BindingFlags.NonPublic |
                                                             BindingFlags.Instance;

        public static readonly BindingFlags PublicStatic = BindingFlags.Public |
                                                           BindingFlags.Static;

        public static readonly BindingFlags NoPublicStatic = BindingFlags.NonPublic |
                                                             BindingFlags.Static;
    }
}