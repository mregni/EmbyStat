using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EmbyStat.Common.Helpers
{
    public interface IJsonSerializer
    {
        T DeserializeFromStream<T>(Stream stream);
        object DeserializeFromStream(Stream stream, Type type);
        T DeserializeFromString<T>(string text);
        object DeserializeFromString(string json, Type type);
        void SerializeToStream(object obj, Stream stream);
        string SerializeToString(object obj);
    }
}
