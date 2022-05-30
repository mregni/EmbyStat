using System;
using System.Collections.Generic;

namespace EmbyStat.Common.Models;

[AttributeUsage(AttributeTargets.Property)]
public class OptionSerializerAttribute : Attribute
{
    private string Key { get; }

    public OptionSerializerAttribute(string key)
    {
        Key = key;
    }

    public KeyValuePair<string, string> ToKeyValuePair<T>(T value)
    {
        return new KeyValuePair<string, string>(Key, value.ToString());
    }
}