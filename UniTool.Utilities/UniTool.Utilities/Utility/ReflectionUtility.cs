using System;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace UniTool.Utilities
{
    public static class ReflectionUtility
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
                sb.Append($"({GetMethodParametersSignature(methodInfo)}) : ");
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


        public static string GetMethodParametersSignature(MethodInfo method)
        {
            return string.Join(", ",
                method.GetParameters().Select(x => $"{TypeUtility.GetAliases(x.ParameterType)} {x.Name}"));
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
        
        public static object GetObjectValue(object obj, string name, BindingFlags flags)
        {
            var t = obj.GetType();
            var f = t.GetField(name, flags);
            if (f != null)
            {
                return f.GetValue(obj);
            }
            var p = t.GetProperty(name, flags);
            if (p != null)
            {
                return p.GetValue(obj);
            }

            throw new ArgumentException($"No field or property name:{name}");
        }
        
        public static Type GetObjectValueType(object obj, string name, BindingFlags flags)
        {
            var t = obj.GetType();
            var f = t.GetField(name, flags);
            if (f != null)
            {
                return f.FieldType;
            }
            var p = t.GetProperty(name, flags);
            if (p != null)
            {
                return p.PropertyType;
            }

            throw new ArgumentException($"No field or property name:{name}");
        }
        
        public static void SetObjectValue(object obj, string name, object val, BindingFlags flags)
        {
            var t = obj.GetType();
            var f = t.GetField(name, flags);
            if (f != null)
            {
                f.SetValue(obj, val);
            }
            var p = t.GetProperty(name, flags);
            if (p != null)
            {
                p.SetValue(obj, val);
            }

            throw new ArgumentException($"No field or property name:{name}");
        }

        // public static MethodInfo GetMethodEx(Type targetType, string methodName,
        //     IEnumerable<string> parameterDecls, BindingFlags bindingFlags)
        // {
        //     var paramTypesName = parameterDecls.Select(param => param.Trim().Split(' ')[0]).ToArray();
        //
        //     var method = targetType.GetMethods(bindingFlags).FirstOrDefault(x =>
        //     {
        //         if (x.Name != methodName)
        //             return false;
        //         var parameters = x.GetParameters();
        //         if (parameters.Length != paramTypesName.Length)
        //             return false;
        //         return !parameters
        //             .Where((t, i) => t.ParameterType.Name != paramTypesName[i])
        //             .Any();
        //     });
        //     return method;
        // }
    }
}