using System;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace UniTool.Helper
{
    public static class ReflectHelper
    {
        public static bool IsInherit<T>(this Type t)
        {
            return typeof(T).IsAssignableFrom(t);
        }


        public static object DefaultInstance(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            if (type == typeof(string))
                return string.Empty;
            return null;
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

        public static string GetSignature(MemberInfo member)
        {
            var sb = new StringBuilder();

            // Append the member type (e.g., Method, Property, Field, etc.)
            sb.Append(member.MemberType.ToString());
            sb.Append(" ");

            // Append the member's declaring type (including namespace)
            sb.Append(member.DeclaringType.FullName);
            sb.Append(".");

            // Append the member name
            sb.Append(member.Name);

            // If the member is a method, append parameter types and return type
            if (member is MethodInfo methodInfo)
            {
                sb.Append("(");
                var parameters = methodInfo.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    sb.Append(parameters[i].ParameterType.FullName);
                }
                sb.Append(")");
                sb.Append(" : ");
                sb.Append(methodInfo.ReturnType.FullName);
            }
            else if (member is PropertyInfo propertyInfo)
            {
                // If the member is a property, append the property type
                sb.Append(" : ");
                sb.Append(propertyInfo.PropertyType.FullName);
            }
            else if (member is FieldInfo fieldInfo)
            {
                // If the member is a field, append the field type
                sb.Append(" : ");
                sb.Append(fieldInfo.FieldType.FullName);
            }
            else if (member is EventInfo eventInfo)
            {
                // If the member is an event, append the event handler type
                sb.Append(" : ");
                sb.Append(eventInfo.EventHandlerType.FullName);
            }

            return sb.ToString();
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

        public static bool HasCustomAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes<T>().Any();
        }

        public static bool HasCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            return member.GetCustomAttributes<T>().Any();
        }


        public static bool TryGetValueType(MemberInfo member, out Type type)
        {
            if (member is MethodInfo m)
            {
                type = m.ReturnType;
            }
            else if (member is FieldInfo f)
            {
                type = f.FieldType;
            }
            else if (member is PropertyInfo p)
            {
                type = p.PropertyType;
            }
            else
            {
                type = null;
                return false;
            }
            return true;
        }

        public static readonly TypeCode[] IntegerTypes =
        {
            TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32,
            TypeCode.Int64, TypeCode.UInt64
        };

        public static bool IsIntegerType(Type type)
        {
            return typeof(int).IsAssignableFrom(type);
        }

        public static readonly TypeCode[] FloatingPointTypes =
        {
            TypeCode.Double, TypeCode.Single, TypeCode.Decimal
        };

        public static bool IsFloatType(Type type)
        {
            return typeof(float).IsAssignableFrom(type);
        }

        public static bool IsBoolType(Type type)
        {
            return typeof(bool).IsAssignableFrom(type);
        }

        public static bool IsStringType(Type type)
        {
            return typeof(string).IsAssignableFrom(type);
        }

        public static bool IsUnityObjectType(Type type)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(type);
        }

        public static bool IsVisualType(Type type)
        {
            return type.IsPrimitive || IsUnityObjectType(type);
        }

        public static MethodInfo GetMethodEx(Type targetType, string methodName,
            IEnumerable<string> parameterDecls, BindingFlags bindingFlags)
        {
            var paramTypesName = parameterDecls.Select(param => param.Trim().Split(' ')[0]).ToArray();

            var method = targetType.GetMethods(bindingFlags).FirstOrDefault(x =>
            {
                if (x.Name != methodName)
                    return false;
                var parameters = x.GetParameters();
                if (parameters.Length != paramTypesName.Length)
                    return false;
                return !parameters
                    .Where((t, i) => t.ParameterType.Name != paramTypesName[i])
                    .Any();
            });
            return method;
        }
    }
}