namespace EmbyStat.Core.Disk.Interfaces;

public interface IDiskProvider
{
    string[] GetFiles(string path, SearchOption searchOption);
    string ReadAllText(string filePath);
    bool FolderExists(string path);
}