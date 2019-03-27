namespace EmbyStat.Clients.GitHub.Models
{
    public class UpdateResult
    {
        public UpdateResult()
        {
            IsUpdateAvailable = false;
            Package = new PackageInfo();
        }
        public bool IsUpdateAvailable { get; set; }
        public string AvailableVersion { get; set; }
        public PackageInfo Package { get; set; }
    }
}
