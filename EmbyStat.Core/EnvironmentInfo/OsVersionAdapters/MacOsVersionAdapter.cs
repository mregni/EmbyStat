﻿using System.Text.RegularExpressions;
using EmbyStat.Core.Disk.Interfaces;
using EmbyStat.Core.EnvironmentInfo.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Core.EnvironmentInfo.OsVersionAdapters;

public class MacOsVersionAdapter: IOsVersionAdapter
{
    private const string PLIST_DIR = "/System/Library/CoreServices/";

    private static readonly Regex DarwinVersionRegex = new Regex("<string>(?<version>10\\.\\d{1,2}\\.?\\d{0,2}?)<\\/string>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly IDiskProvider _diskProvider;
    private readonly ILogger<MacOsVersionAdapter> _logger;

    public MacOsVersionAdapter(IDiskProvider diskProvider, ILogger<MacOsVersionAdapter> logger)
    {
        _diskProvider = diskProvider;
        _logger = logger;
    }

    public OsVersionModel Read()
    {
        if (!OperatingSystem.IsMacOS() && !OperatingSystem.IsMacCatalyst())
        {
            return null;
        }
        
        var version = "10.0";

        if (!_diskProvider.FolderExists(PLIST_DIR))
        {
            _logger.LogDebug("Directory {0} doesn't exist", PLIST_DIR);
            return null;
        }

        var allFiles = _diskProvider.GetFiles(PLIST_DIR, SearchOption.TopDirectoryOnly);

        var versionFile = allFiles.SingleOrDefault(c =>
            c.EndsWith("/SystemVersion.plist") ||
            c.EndsWith("/ServerVersion.plist"));

        if (string.IsNullOrWhiteSpace(versionFile))
        {
            _logger.LogDebug("Couldn't find version plist file in {0}", PLIST_DIR);
            return null;
        }

        var text = _diskProvider.ReadAllText(versionFile);
        var match = DarwinVersionRegex.Match(text);

        if (match.Success)
        {
            version = match.Groups["version"].Value;
        }

        var name = versionFile.Contains("Server") ? "macOS Server" : "macOS";

        return new OsVersionModel(name, version);
    }

    public bool Enabled => OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst();
}