using Newtonsoft.Json;

namespace UniTool.Json
{
    public static class JsonSettings
    {
        public static JsonSerializerSettings Serializer = new JsonSerializerSettings();

        static JsonSettings()
        {
            Serializer.Converters.Add(new Vector2Converter());
            Serializer.Converters.Add(new RectConverter());
        }
    }
}