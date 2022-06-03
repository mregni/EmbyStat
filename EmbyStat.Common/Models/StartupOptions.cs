using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

namespace EmbyStat.Common.Models;

public class StartupOptions
{
    [Option('p', "port", Required = false, Default = null, HelpText = "Set the port EmbyStat needs to be hosted on.")]
    [OptionSerializer("Hosting:Port")]
    public int? Port { get; set; }

    [Option('n', "enable-updates", Required = false, Default = null, HelpText = "Disable all update flow and pages on the server.")]
    [OptionSerializer("SystemConfig:CanUpdate")]
    public bool? NoUpdates { get; set; }

    [Option('d', "data-dir", Required = false, Default = "data", HelpText = "Folder where database is stored, default is /data")]
    [OptionSerializer("SystemConfig:Dirs:Data", true)]
    public string DataDir { get; set; }

    [Option('l', "log-dir", Required = false, Default = "logs", HelpText = "Folder where log files are stored, default is /logs")]
    [OptionSerializer("SystemConfig:Dirs:Logs", true)]
    public string LogDir { get; set; }

    [Option('c', "config-dir", Required = false, Default = "config", HelpText = "Folder where config files are stored, default is /config")]
    [OptionSerializer("SystemConfig:Dirs:Config", true)]
    public string ConfigDir { get; set; }

    [Option('g', "log-level", Required = false, Default = null, HelpText = "Set the proper log level\n1: Debug\n2: Information")]
    [OptionSerializer("LogLevel")]
    public int? LogLevel { get; set; }

    [Option('u', "listen-urls", Required = false, Default = "", HelpText = "Set the url's where EmbyStat needs to listen on. Default is http://* but you can change it to only loopback address here if needed. Don't add port number, it will be added automatically. Use ; if you want to define more then one url.")]
    [OptionSerializer("Hosting:Urls")]
    public string ListeningUrls { get; set; }

    [Option('s', "service", Required = false, Default = null, HelpText = "Indicate EmbyStat is running as a service")]
    [OptionSerializer("Hosting:Urls")]
    public bool? RunAsService { get; set; }

    public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
    {
        var type = GetType();
        var properties = type
            .GetProperties()
            .Where(prop => prop.IsDefined(typeof(OptionSerializerAttribute), false));;

        foreach (var prop in properties)
        {
            var value = prop.GetValue(this, null);
            var attribute = (OptionSerializerAttribute)prop
                .GetCustomAttributes(typeof(OptionSerializerAttribute), false)
                .First();
            
            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            var dataType = propType.Name;
            
            if (dataType == nameof(String) && !string.IsNullOrWhiteSpace((string)value))
            {
                yield return attribute.ToKeyValuePair(value); 
            }

            if (dataType == nameof(Boolean) && value != null)
            {
                yield return attribute.ToKeyValuePair(value);
            }
            
            if (dataType == nameof(Int32) && value != null)
            {
                yield return attribute.ToKeyValuePair(value);
            }
            
        }
    }
}