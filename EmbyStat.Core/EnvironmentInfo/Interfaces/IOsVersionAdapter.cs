namespace EmbyStat.Core.EnvironmentInfo.Interfaces;

public interface IOsVersionAdapter
{
    bool Enabled { get; }
    OsVersionModel Read();
}