using System;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization;

namespace EmbyStat.Common.Net
{
    public class JsonNetSerializer : IRestSerializer
    {
        public string Serialize(object obj) =>
            JsonConvert.SerializeObject(obj);

        public string Serialize(Parameter parameter) =>
            JsonConvert.SerializeObject(parameter.Value);

        public T Deserialize<T>(IRestResponse response)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(response.Content, typeof(T));
            }

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public string[] SupportedContentTypes { get; } =
        {
            "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
        };

        public string ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;
    }
}
