using EmbyStat.Core.EnvironmentInfo.Interfaces;

namespace EmbyStat.Core.EnvironmentInfo;

public class OsInfo : IOsInfo
{
    public static bool IsLinux => OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD();
    public static bool IsOsx => OperatingSystem.IsMacCatalyst() || OperatingSystem.IsMacOS();
    public static bool IsWindows => OperatingSystem.IsWindows();

    // this needs to not be static so we can mock it
    public bool IsDocker { get; }
    public string Version { get; }
    public string Name { get; }
    public string FullName { get; }

    public OsInfo(IEnumerable<IOsVersionAdapter> versionAdapters)
    {
        OsVersionModel osInfo = null;

        foreach (var osVersionAdapter in versionAdapters.Where(c => c.Enabled))
        {
            try
            {
                osInfo = osVersionAdapter.Read();
            }
            catch (Exception e)
            {
                // ignored
            }

            if (osInfo != null)
            {
                break;
            }
        }

        if (osInfo != null)
        {
            Name = osInfo.Name;
            Version = osInfo.Version;
            FullName = osInfo.FullName;
        }
        else
        {
            Name = Environment.OSVersion.VersionString;
            FullName = Name;
        }

        if (IsLinux && File.Exists("/proc/1/cgroup") && File.ReadAllText("/proc/1/cgroup").Contains("/docker/"))
        {
            IsDocker = true;
        }
    }
}