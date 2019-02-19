using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models.Entities;
using MediaBrowser.Model.IO;

namespace EmbyStat.Common.Converters
{
    public static class DriveConverter
    {
        public static IEnumerable<Drive> ConvertToDeviceList(IEnumerable<FileSystemEntryInfo> entries)
        {
            foreach (var entry in entries)
            {
                yield return new Drive
                {
                    Name = entry.Name,
                    Path = entry.Path
                };
            }
        }
    }
}
