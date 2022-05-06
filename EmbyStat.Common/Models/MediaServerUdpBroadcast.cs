using Newtonsoft.Json;

namespace EmbyStat.Common.Models;

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
            int portNumber = 0;
            if (list.Length > 1)
            {
                if (int.TryParse(list[list.Length - 1], out portNumber))
                {
                    Port = portNumber;
                }
                else
                {
                    var subList = list[list.Length - 1].Split('/');
                    if (int.TryParse(subList[0], out portNumber))
                    {
                        Port = portNumber;
                        if (subList.Length > 1)
                        {
                            BaseUrl = subList[1];
                        }
                    }
                }
            }

            if (Protocol == 1)
            {
                _address = value.ToLowerInvariant()
                    .Replace("http://", string.Empty)
                    .Replace($":{portNumber}", string.Empty)
                    .Replace($"/{BaseUrl}", string.Empty);
            }
            else
            {
                _address = value.ToLowerInvariant()
                    .Replace("https://", string.Empty)
                    .Replace($":{portNumber}", string.Empty)
                    .Replace($"/{BaseUrl}", string.Empty);
            }
        }
    }

    public string Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public int Port { get; set; }
    [JsonIgnore]
    public int Protocol { get; set; }
    private string BaseUrl { get; set; }
}