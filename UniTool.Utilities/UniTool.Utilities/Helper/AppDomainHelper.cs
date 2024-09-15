using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UniTool.Utilities
{
    public static class AppDomainHelper
    {
        public static IEnumerable<Assembly> GetProjectAssemblies(AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("Assembly."));
        }

        public static IEnumerable<Assembly> GetUnityEngineAssemblies(AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("UnityEngine."));
        }

        public static IEnumerable<Assembly> GetUnityEditorAssemblies(AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("UnityEditor."));
        }

        public static IEnumerable<Assembly> GetUnityAssemblies(AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("Unity."));
        }

        public static IEnumerable<Assembly> GetSystemAssemblies(AppDomain domain)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("System."));
        }
    }
}