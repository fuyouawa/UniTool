using System;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace UniTool.Utilities
{
    public static class ReflectHelper
    {
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