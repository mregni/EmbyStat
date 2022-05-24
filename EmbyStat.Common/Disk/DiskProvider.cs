using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmbyStat.Common.Disk.Interfaces;
using EmbyStat.Common.EnvironmentInfo;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Common.Disk
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
