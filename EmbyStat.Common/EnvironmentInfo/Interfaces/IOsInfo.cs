namespace EmbyStat.Common.EnvironmentInfo;

public interface IOsInfo
{
    string Version { get; }
    string Name { get; }
    string FullName { get; }
    bool IsDocker { get; }
}