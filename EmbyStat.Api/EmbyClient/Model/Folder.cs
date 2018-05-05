using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Api.EmbyClient.Model
{
    public class Folder
    {
        public string Name { get; set; }
        public string ServerId { get; set; }
        public string Id { get; set; }
        public string Etag { get; set; }
        public DateTime DateCreated { get; set; }
        public bool CanDelete { get; set; }
        public bool CanDownload { get; set; }
        public bool SupportsSync { get; set; }
        public string SortName { get; set; }
        public string Path { get; set; }
        public string PlayAccess { get; set; }
        public bool IsFolder { get; set; }
        public string Type { get; set; }
        public int ChildCount { get; set; }
        public string DisplayPreferencesId { get; set; }
        public string LocationType { get; set; }
        public bool LockData { get; set; }
    }
}
