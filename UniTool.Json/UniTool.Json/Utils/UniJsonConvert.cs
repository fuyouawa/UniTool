using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UniTool.Utilities;
using UnityEngine;

namespace UniTool.Json
{
    public static class UniJsonConvert
    {
        public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();

        static UniJsonConvert()
        {
            SerializerSettings.Converters.Add(new ColorConverter());
            SerializerSettings.Converters.Add(new RectConverter());
            SerializerSettings.Converters.Add(new Vector2Converter());
            SerializerSettings.Converters.Add(new Vector2IntConverter());
            SerializerSettings.Converters.Add(new KeyframeConverter());
            SerializerSettings.Converters.Add(new AnimationCurveConverter());
        }

        public static void AddConverter(JsonConverter item)
        {
            SerializerSettings.Converters.Add(item);
        }

        public static string ToJson<T>(T value)
        {
            return JsonConvert.SerializeObject(value, SerializerSettings);
        }

        public static string ToJson<T>(T value, Formatting formatting)
        {
            return JsonConvert.SerializeObject(value, formatting, SerializerSettings);
        }

        public static string ToJson<T>(T value, params JsonConverter[] converts)
        {
            var s = new JsonSerializerSettings(SerializerSettings);
            s.Converters.AddRange(converts);
            return JsonConvert.SerializeObject(value, s);
        }

        public static string ToJson<T>(T value, Formatting formatting, params JsonConverter[] converts)
        {
            var s = new JsonSerializerSettings(SerializerSettings);
            s.Converters.AddRange(converts);
            return JsonConvert.SerializeObject(value, formatting, s);
        }

        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, SerializerSettings);
        }

        public static T FromJson<T>(string json, params JsonConverter[] converts)
        {
            var s = new JsonSerializerSettings(SerializerSettings);
            s.Converters.AddRange(converts);
            return JsonConvert.DeserializeObject<T>(json, s);
        }
    }
}