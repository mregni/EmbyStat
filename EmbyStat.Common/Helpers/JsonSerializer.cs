using System;
using System.IO;
using Newtonsoft.Json;

namespace EmbyStat.Common.Helpers
{
    public class JsonSerializer : IJsonSerializer
    {
        public void SerializeToStream(object obj, Stream stream)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            using (var jsonWriter = new JsonTextWriter(new StreamWriter(stream)))
            {
                Newtonsoft.Json.JsonSerializer.Create(new JsonSerializerSettings()).Serialize(jsonWriter, obj);
            }
        }

        public object DeserializeFromStream(Stream stream, Type type)
        {
            using (var jsonReader = new JsonTextReader(new StreamReader(stream)))
            {
                return Newtonsoft.Json.JsonSerializer.Create(new JsonSerializerSettings()).Deserialize(jsonReader, type);
            }
        }

        public T DeserializeFromStream<T>(Stream stream)
        {
            return (T)DeserializeFromStream(stream, typeof(T));
        }

        public T DeserializeFromString<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public object DeserializeFromString(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public string SerializeToString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public byte[] SerializeToBytes(object obj)
        {
            string serialized = SerializeToString(obj);
            return global::System.Text.Encoding.UTF8.GetBytes(serialized);
        }
    }
}
