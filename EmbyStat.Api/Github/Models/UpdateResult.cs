namespace EmbyStat.Api.Github.Models
{
    public class UpdateResult
    {
        public UpdateResult()
        {
            IsUpdateAvailable = false;
        }
        public bool IsUpdateAvailable { get; set; }
        public string AvailableVersion { get; set; }
        public PackageInfo Package { get; set; }
    }
}
