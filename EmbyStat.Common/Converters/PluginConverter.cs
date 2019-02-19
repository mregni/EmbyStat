using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Common.Converters
{
    public static class PluginConverter
    {
        public static IEnumerable<PluginInfo> ConvertToPluginList(IEnumerable<MediaBrowser.Model.Plugins.PluginInfo> plugins)
        {
            foreach (var plugin in plugins)
            {
                yield return new PluginInfo
                {
                    Id = plugin.Id,
                    Name = plugin.Name,
                    Description = plugin.Description,
                    Version = plugin.Version,
                    ConfigurationFileName = plugin.ConfigurationFileName
                };
            }
        }
    }
}
