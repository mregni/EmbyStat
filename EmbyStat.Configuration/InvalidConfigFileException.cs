namespace EmbyStat.Configuration;

public class InvalidConfigFileException : Exception
{
    public InvalidConfigFileException(string message)
        : base(message)
    {
    }
}