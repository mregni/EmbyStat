namespace EmbyStat.Core.EnvironmentInfo.Interfaces;

public interface IOsInfo
{
    string Version { get; }
    string Name { get; }
    string FullName { get; }
    bool IsDocker { get; }
}