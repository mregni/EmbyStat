using Newtonsoft.Json;

namespace EmbyStat.Common.Models
{
    public class MediaServerUdpBroadcast
    {
        [JsonIgnore]
        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                Protocol = value.ToLowerInvariant().StartsWith("https") ? 0 : 1;

                var list = value.Split(':');
                if (int.TryParse(list[list.Length - 1], out var portNumber))
                {
                    Port = portNumber;
                }

                if (Protocol == 1)
                {
                    _address = value.ToLowerInvariant()
                        .Replace("http://", string.Empty)
                        .Replace($":{portNumber}", string.Empty);
                }
                else
                {
                    _address = value.ToLowerInvariant()
                        .Replace("https://", string.Empty)
                        .Replace($":{portNumber}", string.Empty);
                }
            }
        }

        public string Id { get; set; }
	    public string Name { get; set; }
        [JsonIgnore]
        public int Port { get; set; }
        [JsonIgnore]
        public int Protocol { get; set; }
	}
}
