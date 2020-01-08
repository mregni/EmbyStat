using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Net;
using RestSharp;

namespace EmbyStat.Clients.Base.Http
{
    public class BaseHttpClient
    {
        protected string DeviceName { get; set; }
        protected string ApplicationVersion { get; set; }
        protected string DeviceId { get; set; }

        protected string apiKey { get; set; }
        public string ApiKey
        {
            get => apiKey;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                apiKey = value;
            }
        }

        public string BaseUrl
        {
            get => Client.BaseUrl?.ToString() ?? string.Empty;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                Client.BaseUrl = new Uri(value);
            }
        }

        protected string AuthorizationScheme { get; set; }
        protected string AuthorizationParameter => $"Client=\"other\", DeviceId=\"{DeviceId}\", Device=\"{DeviceName}\", Version=\"{ApplicationVersion}\"";

        protected readonly IRestClient Client;

        public BaseHttpClient(IRestClient client)
        {
            Client = client;
            client.UseSerializer(() => new JsonNetSerializer());
        }

        public void SetDeviceInfo(string deviceName, string authorizationScheme, string applicationVersion, string deviceId)
        {
            AuthorizationScheme = authorizationScheme;
            ApplicationVersion = applicationVersion;
            DeviceId = deviceId;
            DeviceName = deviceName;
        }
    }
}
