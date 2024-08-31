using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniTool.Utilities;
using UnityEngine;

namespace UniTool.Json
{
    public static class Extension
    {
        private static JTokenType GetJTokenType(Type type)
        {
            if (type.IsStringType())
                return JTokenType.String;
            if (type.IsIntegerType())
                return JTokenType.Integer;
            if (type.IsFloatType())
                return JTokenType.Float;
            if (type.IsBoolType())
                return JTokenType.Boolean;
            if (type == typeof(DateTime))
                return JTokenType.Date;
            if (type == typeof(Guid))
                return JTokenType.Guid;
            if (type == typeof(Uri))
                return JTokenType.Uri;
            if (type == typeof(TimeSpan))
                return JTokenType.TimeSpan;
            if (type.IsArray || typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
                return JTokenType.Array;
            if (type.IsClass || type.IsValueType)
                return JTokenType.Object;

            return JTokenType.None;
        }

        public static void WriteProperty(this JsonWriter writer, string propertyName, object val)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteValue(val);
        }


        public static bool TryGetProperty<T>(this JObject j, string propertyName, out T val)
        {
            if (j.TryGetValue(propertyName, out var t))
            {
                if (t.Type == GetJTokenType(typeof(T)))
                {
                    val = t.ToObject<T>();
                    return true;
                }
            }

            val = default;
            return false;
        }


        public static T GetProperty<T>(this JObject j, string propertyName)
        {
            if (j.TryGetValue(propertyName, out var t))
            {
                if (t.Type == GetJTokenType(typeof(T)))
                {
                    return t.ToObject<T>();
                }
            }

            throw new ArgumentException($"No property:{propertyName}");
        }

        public static JsonWriteObjectScope WriteObjectScope(this JsonWriter writer)
        {
            return new JsonWriteObjectScope(writer);
        }

        public static JsonWriteArrayScope WriteArrayScope(this JsonWriter writer)
        {
            return new JsonWriteArrayScope(writer);
        }
    }
}