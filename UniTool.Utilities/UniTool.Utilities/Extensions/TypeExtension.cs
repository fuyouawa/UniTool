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

        public static T GetPropertyValue<T>(this Type type, string propertyName, BindingFlags flags, object target)
        {
            var property = type.GetProperty(propertyName, flags);
            if (property == null)
            {
                throw new ArgumentException($"类型:\"{type.FullName}\"没有BindingFlags为:{flags}的属性:\"{propertyName}\"");
            }
            return (T)property.GetValue(target, null);
        }

        public static T GetPropertyValue<T>(this Type type, string propertyName, object target)
        {
            return type.GetPropertyValue<T>(propertyName, ReflectionUtility.AllBindingFlags, target);
        }

        public static MethodInfo GetMethodEx(this Type type, string methodName, BindingFlags flags, Type[] argTypes)
        {
            return type.GetMethods(flags).FirstOrDefault(m =>
            {
                if (m.Name != methodName)
                {
                    return false;
                }
                var parameters = m.GetParameters();
                if (argTypes == null)
                {
                    return parameters.Length == 0;
                }

                if (argTypes.Length != parameters.Length)
                {
                    return false;
                }
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (!parameters[i].ParameterType.IsAssignableFrom(argTypes[i]))
                    {
                        return false;
                    }
                }
                return true;
            });
        }

        public static object InvokeMethod(this Type type, string methodName, BindingFlags flags, object target, params object[] args)
        {
            var method = type.GetMethodEx(methodName, flags, args.Select(a => a.GetType()).ToArray());

            if (method == null)
            {
                throw new ArgumentException($"类型\"{type}\"中没有名为\"{methodName}\"并且\"{flags}\"的函数!");
            }
            return method.Invoke(target, args);
        }

        public static object InvokeMethod(this Type type, string methodName, object target,
            params object[] args)
        {
            return type.InvokeMethod(methodName, ReflectionUtility.AllBindingFlags, target, args);
        }

        public static void AddEvent(this Type type, string eventName, BindingFlags flags, object target, Delegate func)
        {
            var e = type.GetEvent(eventName, flags);
            if (e == null)
            {
                throw new ArgumentException($"类型\"{type}\"中没有名为\"{eventName}\"并且\"{flags}\"的事件!");
            }

            e.GetAddMethod().Invoke(target, new object[] { func });
        }

        public static void AddEvent(this Type type, string eventName, object target, Delegate func)
        {
            type.AddEvent(eventName, ReflectionUtility.AllBindingFlags, target, func);
        }
    }
}