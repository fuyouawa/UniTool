using System;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UniTool.Extension
{
    public static class ReflectExtension
    {
        public static bool IsInherit<T>(this Type t)
        {
            return typeof(T).IsAssignableFrom(t);
        }

        public static string GetSignature(this MethodInfo method)
        {
            var ns = method.DeclaringType.Namespace;
            var cn = method.DeclaringType.Name;
            var mn = method.Name;
            var rn = method.ReturnType;

            var ps = method.GetParameters();

            var sb = new StringBuilder();

            foreach (var p in ps)
            {
                sb.Append($"{p.ParameterType.Name} {p.Name}, ");
            }

            if (ps.Length > 0)
            {
                sb.Remove(sb.Length - 2, 2);
            }

            return $"{ns}.{cn}.{mn}({sb}) : {rn}";
        }

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
    }
}