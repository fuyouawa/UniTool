using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

namespace UniTool.Utilities
{
    public static class TypeExtension
    {
        public static bool IsInherit<T>(this Type t)
        {
            return typeof(T).IsAssignableFrom(t);
        }


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


        private static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string>
        {
            { typeof(void), "void" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(object), "object" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(string), "string" }
        };

        public static string GetAliases(this Type t)
        {
            if (TypeAliases.TryGetValue(t, out string alias))
            {
                return alias;
            }

            // If not found in the alias dictionary, return the full name without the namespace
            return t.IsGenericType ? GetGenericTypeName(t) : t.Name;
        }


        private static string GetGenericTypeName(Type type)
        {
            var genericArguments = type.GetGenericArguments();
            var typeName = type.Name;
            var genericPartIndex = typeName.IndexOf('`');
            if (genericPartIndex > -1)
            {
                typeName = typeName.Substring(0, genericPartIndex);
            }

            var genericArgs = string.Join(", ", Array.ConvertAll(genericArguments, t => t.GetAliases()));
            return $"{typeName}<{genericArgs}>";
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