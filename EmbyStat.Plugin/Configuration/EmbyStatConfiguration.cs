using System.Collections.Generic;

namespace EmbyStat.Plugin.Configuration;

public class EmbyStatConfiguration
{
    public string EmbyStatUrl { get; set; }
    public List<string> LibraryTypesToScan { get; set; }
}