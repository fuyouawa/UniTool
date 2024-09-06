using System;
using Newtonsoft.Json;
using UniTool.Utilities;
using UnityEngine;

namespace UniTool.Json
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteValue("#" + ColorUtility.ToHtmlStringRGBA(value));
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            ColorUtility.TryParseHtmlString((string)reader.Value, out var color);
            return color;
        }
    }
}