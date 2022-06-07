using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace EmbyStat.Common.Models;

[AttributeUsage(AttributeTargets.Property)]
public class OptionSerializerAttribute : Attribute
{
    private string Key { get; }
    private bool NeedsCurrentDirPrefix { get; }

    public OptionSerializerAttribute(string key)
    {
        Key = key;
    }
    
    public OptionSerializerAttribute(string key, bool needsCurrentDirPrefix)
    {
        Key = key;
        NeedsCurrentDirPrefix = needsCurrentDirPrefix;
    }

    public KeyValuePair<string, string> ToKeyValuePair<T>(T value)
    {
        if (NeedsCurrentDirPrefix && value != null)
        {
            var path = value.Cast<string>();
            if (path.StartsWith('/'))
            {
                path = path.Remove(0, 1);
            }
            
            path = Path.Combine(Directory.GetCurrentDirectory(), path);
            return new KeyValuePair<string, string>(Key, path);
        }
        
        return new KeyValuePair<string, string>(Key, value?.ToString());
    }
}