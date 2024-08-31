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
    }
}