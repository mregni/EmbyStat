namespace EmbyStat.Core.Processes;

public class ProcessOutputLine
{
    public ProcessOutputLevel Level { get; set; }
    public string Content { get; set; }
    public DateTime Time { get; set; }

    public ProcessOutputLine(ProcessOutputLevel level, string content)
    {
        Level = level;
        Content = content;
        Time = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"{Time} - {Level} - {Content}";
    }
}