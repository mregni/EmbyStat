using System;
using System.IO;
using System.Threading.Tasks;
using MediaBrowser.Model.Serialization;
using Newtonsoft.Json;

namespace EmbyStat.Api.EmbyClient
{
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        public void SerializeToStream(object obj, Stream stream)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            using (var jsonWriter = new JsonTextWriter(new StreamWriter(stream)))
            {
                JsonSerializer.Create(new JsonSerializerSettings()).Serialize(jsonWriter, obj);
            }
        }

        public object DeserializeFromStream(Stream stream, Type type)
        {
            using (var jsonReader = new JsonTextReader(new StreamReader(stream)))
            {
                return JsonSerializer.Create(new JsonSerializerSettings()).Deserialize(jsonReader, type);
            }
        }

        public Task<object> DeserializeFromStreamAsync(Stream stream, Type type)
        {
            throw new NotImplementedException();
        }

        public T DeserializeFromStream<T>(Stream stream)
        {
            return (T)DeserializeFromStream(stream, typeof(T));
        }

        public Task<T> DeserializeFromStreamAsync<T>(Stream stream)
        {
            throw new NotImplementedException();
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

        public void SerializeToFile(object obj, string file)
        {
            throw new NotImplementedException();
        }

        public object DeserializeFromFile(Type type, string file)
        {
            throw new NotImplementedException();
        }

        public T DeserializeFromFile<T>(string file) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
