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
            writer.WriteValue(value.ToHex());
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return ColorHelper.FromHex((string)reader.Value);
        }
    }
}