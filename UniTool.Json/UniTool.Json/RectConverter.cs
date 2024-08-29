using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UniTool.Json
{
    public class RectConverter : JsonConverter<Rect>
    {
        public override void WriteJson(JsonWriter writer, Rect value, JsonSerializer serializer)
        {
            // 将 Rect 序列化为 { "x": value.x, "y": value.y, "width": value.width, "height": value.height } 格式
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("width");
            writer.WriteValue(value.width);
            writer.WritePropertyName("height");
            writer.WriteValue(value.height);
            writer.WriteEndObject();
        }

        public override Rect ReadJson(JsonReader reader, Type objectType, Rect existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            // 从 { "x": value.x, "y": value.y, "width": value.width, "height": value.height } 反序列化为 Rect 对象
            var obj = JObject.Load(reader);
            var x = (float)obj["x"];
            var y = (float)obj["y"];
            var width = (float)obj["width"];
            var height = (float)obj["height"];
            return new Rect(x, y, width, height);
        }
    }
}
