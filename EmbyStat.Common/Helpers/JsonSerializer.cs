using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace EmbyStat.Common.Helpers
{
    public static class JsonSerializerExtentions
    {
        public static void SerializeToStream(object obj, Stream stream)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            using (var jsonWriter = new JsonTextWriter(new StreamWriter(stream)))
            {
                JsonSerializer.Create(new JsonSerializerSettings()).Serialize(jsonWriter, obj);
            }
        }

        public static object DeserializeFromStream(Stream stream, Type type)
        {
            using (var jsonReader = new JsonTextReader(new StreamReader(stream)))
            {
                return JsonSerializer.Create(new JsonSerializerSettings()).Deserialize(jsonReader, type);
            }
        }

        public static T DeserializeFromStream<T>(Stream stream)
        {
            return (T)DeserializeFromStream(stream, typeof(T));
        }

        public static  T DeserializeFromString<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public static object DeserializeFromString(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static string SerializeToString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static byte[] SerializeToBytes(object obj)
        {
            var serialized = SerializeToString(obj);
            return Encoding.UTF8.GetBytes(serialized);
        }
    }
}
