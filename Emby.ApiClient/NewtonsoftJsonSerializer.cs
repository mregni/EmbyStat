using MediaBrowser.Model.Serialization;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Emby.ApiClient
{
    /// <summary>
    /// Class NewtonsoftJsonSerializer
    /// </summary>
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        /// <summary>
        /// Serializes to stream.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        public void SerializeToStream(object obj, Stream stream)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            using (var jsonWriter = new JsonTextWriter(new StreamWriter(stream)))
            {
                JsonSerializer.Create(new JsonSerializerSettings()).Serialize(jsonWriter, obj);
            }
        }

        /// <summary>
        /// Deserializes from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public object DeserializeFromStream(Stream stream, Type type)
        {
            using (var jsonReader = new JsonTextReader(new StreamReader(stream)))
            {
                return JsonSerializer.Create(new JsonSerializerSettings()).Deserialize(jsonReader, type);
            }
        }

        /// <summary>
        /// Deserializes from stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns>``0.</returns>
        public T DeserializeFromStream<T>(Stream stream)
        {
            return (T)DeserializeFromStream(stream, typeof(T));
        }

        /// <summary>
        /// Deserializes from string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">The text.</param>
        /// <returns>``0.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T DeserializeFromString<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        /// <summary>
        /// Deserializes from string.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object DeserializeFromString(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        /// <summary>
        /// Serializes to string.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>System.String.</returns>
        public string SerializeToString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Serializes to bytes.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public byte[] SerializeToBytes(object obj)
        {
            string serialized = SerializeToString(obj);
            return System.Text.Encoding.UTF8.GetBytes(serialized);
        }

        /// <summary>
        /// Serializes to file.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="file">The file.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        public void SerializeToFile(object obj, string file)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes from file.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="file">The file.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object DeserializeFromFile(Type type, string file)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">The file.</param>
        /// <returns>``0.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T DeserializeFromFile<T>(string file) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
