using System;
using Newtonsoft.Json;

namespace UniTool.Json
{
    public class JsonWriteObjectScope : IDisposable
    {
        private readonly JsonWriter _writer;

        public JsonWriteObjectScope(JsonWriter writer)
        {
            _writer = writer;
            _writer.WriteStartObject();
        }


        public void Dispose()
        {
            _writer.WriteEndObject();
        }
    }
}