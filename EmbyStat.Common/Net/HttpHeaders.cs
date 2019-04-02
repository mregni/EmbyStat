using System.Collections.Generic;

namespace EmbyStat.Common.Net
{
    public class HttpHeaders : Dictionary<string,string>
    {
        public string AuthorizationScheme { get; set; }
        public string AuthorizationParameter { get; set; }
        public void SetAccessToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Remove("X-MediaBrowser-Token");
            }
            else
            {
                this["X-MediaBrowser-Token"] = token;
            }
        }
    }
}