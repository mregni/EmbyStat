using System.Collections.Generic;

namespace Emby.ApiClient.Net
{
    public class HttpHeaders : Dictionary<string,string>
    {
        /// <summary>
        /// Gets or sets the authorization scheme.
        /// </summary>
        /// <value>The authorization scheme.</value>
        public string AuthorizationScheme { get; set; }
        /// <summary>
        /// Gets or sets the authorization parameter.
        /// </summary>
        /// <value>The authorization parameter.</value>
        public string AuthorizationParameter { get; set; }

        /// <summary>
        /// Sets the access token.
        /// </summary>
        /// <param name="token">The token.</param>
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
