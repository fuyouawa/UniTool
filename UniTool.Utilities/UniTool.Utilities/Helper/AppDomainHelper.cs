using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UniTool.Utilities
{
    public static class AppDomainHelper
    {
        public static IEnumerable<Assembly> GetCustomAssemblies(AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.FullName.StartsWith("UnityEngine.") &&
                            !a.FullName.StartsWith("UnityEditor.") &&
                            !a.FullName.StartsWith("Unity.") &&
                            !a.FullName.StartsWith("System."));
        }

        public static IEnumerable<Type> GetCustomTypes(AppDomain domain)
        {
            return GetCustomAssemblies(domain).SelectMany(a => a.GetTypes());
        }
    }
}