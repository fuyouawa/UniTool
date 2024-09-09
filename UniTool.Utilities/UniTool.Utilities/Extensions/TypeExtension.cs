using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

namespace UniTool.Utilities
{
    public static class TypeExtension
    {
        public static object CreateInstance(this Type type)
        {
            return Activator.CreateInstance(type);
        }


        public static T CreateInstance<T>(this Type type)
        {
            if (!typeof(T).IsAssignableFrom(type))
                throw new ArgumentException($"泛型T({typeof(T).Name})必须可以被参数type({type.Name})转换");
            return (T)CreateInstance(type);
        }

        public static T GetCustomAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes<T>().FirstOrDefault();
        }


        public static bool HasCustomAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes<T>().Any();
        }

        public static bool IsIntegerType(this Type type)
        {
            return typeof(int).IsAssignableFrom(type);
        }

        public static bool IsFloatType(this Type type)
        {
            return typeof(float).IsAssignableFrom(type);
        }

        public static bool IsBoolType(this Type type)
        {
            return typeof(bool).IsAssignableFrom(type);
        }

        public static bool IsStringType(this Type type)
        {
            return typeof(string).IsAssignableFrom(type);
        }

        public static bool IsUnityObjectType(this Type type)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(type);
        }

        public static bool IsVisualType(this Type type)
        {
            return type.IsPrimitive || IsUnityObjectType(type);
        }
    }
}