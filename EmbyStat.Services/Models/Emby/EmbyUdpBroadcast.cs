using System;
using Newtonsoft.Json;

namespace EmbyStat.Services.Models.Emby
{
    public class EmbyUdpBroadcast
    {
        [JsonIgnore]
        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                Protocol = value.ToLowerInvariant().StartsWith("https") ? 1 : 0;

                var list = value.Split(':');
                if (int.TryParse(list[list.Length - 1], out var portNumber))
                {
                    Port = portNumber;
                }

                if (Protocol == 0)
                {
                    _address = value.ToLowerInvariant().Replace("http://", "").Replace($":{Port}", "");
                }
                else
                {
                    _address = value.ToLowerInvariant().Replace("https://", "").Replace($":{Port}", "");
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
