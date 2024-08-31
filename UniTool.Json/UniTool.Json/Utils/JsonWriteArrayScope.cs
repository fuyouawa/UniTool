using System;
using Newtonsoft.Json;

namespace UniTool.Json
{
    public class JsonWriteArrayScope : IDisposable
    {
        private readonly JsonWriter _writer;

        public JsonWriteArrayScope(JsonWriter writer)
        {
            _writer = writer;
            _writer.WriteStartArray();
        }


        public void Dispose()
        {
            _writer.WriteEndArray();
        }
    }
}