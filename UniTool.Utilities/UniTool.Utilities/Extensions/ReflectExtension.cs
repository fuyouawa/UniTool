using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace UniTool.Utilities
{
    public static class ReflectExtension
    {
        public static Delegate CreateDelegate(this MethodInfo method, object target)
        {
            var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

            Type funcType;
            if (method.ReturnType == typeof(void))
            {
                funcType = Expression.GetActionType(parameterTypes);
            }
            else
            {
                funcType = Expression.GetFuncType(parameterTypes.Concat(new[] { method.ReturnType }).ToArray());
            }

            return Delegate.CreateDelegate(funcType, target, method);
        }

        public static bool HasCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            return member.GetCustomAttributes<T>().Any();
        }

        public static IEnumerable<Type> GetAllTypes(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(a => a.GetTypes());
        }

        public static Type FindType(this IEnumerable<Assembly> assemblies, Func<Type, bool> predicate)
        {
            return assemblies.GetAllTypes().FirstOrDefault(predicate);
        }

        public static Type FindTypeByName(this IEnumerable<Assembly> assemblies, string fullName)
        {
            return assemblies.GetAllTypes().FirstOrDefault(t => t.FullName == fullName);
        }

        public static object InvokePrivateMethod(this object target, string methodName, params object[] args)
        {
            return target.GetType().InvokeMethod(methodName, ReflectionUtility.NoPublicBindingFlags, target, args);
        }
    }
}