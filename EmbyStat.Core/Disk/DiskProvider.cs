using EmbyStat.Core.Disk.Interfaces;

namespace EmbyStat.Core.Disk
{
    public class DiskProvider : IDiskProvider
    {
        public string[] GetFiles(string path, SearchOption searchOption)
        {
            return Directory.GetFiles(path, "*.*", searchOption);
        }
        
        public string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }
        
        public bool FolderExists(string path)
        {
            return Directory.Exists(path);
        }
    }
}
