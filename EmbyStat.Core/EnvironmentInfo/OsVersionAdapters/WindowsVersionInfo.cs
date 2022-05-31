using EmbyStat.Core.EnvironmentInfo.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace EmbyStat.Core.EnvironmentInfo.OsVersionAdapters;

public class WindowsVersionInfo: IOsVersionAdapter
{
    private readonly ILogger<WindowsVersionInfo> _logger;
    public bool Enabled => OsInfo.IsWindows;

    public WindowsVersionInfo(ILogger<WindowsVersionInfo> logger)
    {
        _logger = logger;
    }

    public OsVersionModel Read()
    {
        if (!OperatingSystem.IsWindows())
        {
            return null;
        }
        var windowsServer = IsServer();
        var osName = windowsServer ? "Windows Server" : "Windows";
        return new OsVersionModel(osName, Environment.OSVersion.Version.ToString(), Environment.OSVersion.VersionString);
    }

    private bool IsServer()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                const string subkey = @"Software\Microsoft\Windows NT\CurrentVersion";
                var openSubKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                    .OpenSubKey(subkey);
                var productName = openSubKey?.GetValue("ProductName")?.ToString();

                if (productName != null && productName.ToLower().Contains("server"))
                {
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Couldn't detect if running Windows Server");
        }

        return false;
    }
}