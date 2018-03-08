using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Connect;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient.Cryptography;
using Emby.ApiClient.Net;

namespace Emby.ApiClient
{
    public class ConnectService
    {
        internal IJsonSerializer JsonSerializer { get; set; }
        private readonly ILogger _logger;
        private readonly IAsyncHttpClient _httpClient;
        private readonly ICryptographyProvider _cryptographyProvider;
        private readonly string _appName;
        private readonly string _appVersion;

        public ConnectService(IJsonSerializer jsonSerializer, ILogger logger, IAsyncHttpClient httpClient, ICryptographyProvider cryptographyProvider, string appName, string appVersion)
        {
            JsonSerializer = jsonSerializer;
            _logger = logger;
            _httpClient = httpClient;
            _cryptographyProvider = cryptographyProvider;
            _appName = appName;
            _appVersion = appVersion;
        }

        public static string GetConnectPasswordMd5(string password, ICryptographyProvider cryptographyProvider)
        {
            password = ConnectPassword.PerformPreHashFilter(password ?? string.Empty);

            var bytes = Encoding.UTF8.GetBytes(password);

            bytes = cryptographyProvider.CreateMD5(bytes);

            var hash = BitConverter.ToString(bytes, 0, bytes.Length).Replace("-", string.Empty);

            return hash;
        }

        public Task<ConnectAuthenticationResult> Authenticate(string username, string password)
        {
            var md5 = GetConnectPasswordMd5(password ?? string.Empty, _cryptographyProvider);

            var args = new Dictionary<string, string>
            {
                {"nameOrEmail",username},
                {"password",md5}
            };

            return PostAsync<ConnectAuthenticationResult>(GetConnectUrl("user/authenticate"), args);
        }

        public Task Logout(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }

            var args = new Dictionary<string, string>
            {
            };

            return PostAsync<EmptyRequestResult>(GetConnectUrl("user/logout"), args, accessToken);
        }

        public Task<PinCreationResult> CreatePin(string deviceId)
        {
            var args = new Dictionary<string, string>
            {
                {"deviceId",deviceId}
            };

            return PostAsync<PinCreationResult>(GetConnectUrl("pin"), args);
        }

        public async Task<PinStatusResult> GetPinStatus(PinCreationResult pin)
        {
            var dict = new QueryStringDictionary();

            dict.Add("deviceId", pin.DeviceId);
            dict.Add("pin", pin.Pin);

            var url = GetConnectUrl("pin") + "?" + dict.GetQueryString();

            var request = new HttpRequest
            {
                Method = "GET",
                Url = url

            };

            AddAppInfo(request, _appName, _appVersion);

            using (var stream = await _httpClient.SendAsync(request).ConfigureAwait(false))
            {
                return JsonSerializer.DeserializeFromStream<PinStatusResult>(stream);
            }
        }

        public Task<PinExchangeResult> ExchangePin(PinCreationResult pin)
        {
            var args = new Dictionary<string, string>
            {
                {"deviceId",pin.DeviceId},
                {"pin",pin.Pin}
            };

            return PostAsync<PinExchangeResult>(GetConnectUrl("pin/authenticate"), args);
        }

        private async Task<T> PostAsync<T>(string url, Dictionary<string, string> args, string userAccessToken = null)
            where T : class
        {
            var request = new HttpRequest
            {
                Url = url,
                Method = "POST"
            };

            request.SetPostData(args);

            if (!string.IsNullOrEmpty(userAccessToken))
            {
                AddUserAccessToken(request, userAccessToken);
            }

            AddAppInfo(request, _appName, _appVersion);

            using (var stream = await _httpClient.SendAsync(request).ConfigureAwait(false))
            {
                return JsonSerializer.DeserializeFromStream<T>(stream);
            }
        }

        public async Task<ConnectUser> GetConnectUser(ConnectUserQuery query, string accessToken, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }

            var dict = new QueryStringDictionary();

            if (!string.IsNullOrWhiteSpace(query.Id))
            {
                dict.Add("id", query.Id);
            }
            else if (!string.IsNullOrWhiteSpace(query.NameOrEmail))
            {
                dict.Add("nameOrEmail", query.NameOrEmail);
            }
            else if (!string.IsNullOrWhiteSpace(query.Name))
            {
                dict.Add("name", query.Name);
            }
            else if (!string.IsNullOrWhiteSpace(query.Email))
            {
                dict.Add("email", query.Email);
            }
            else
            {
                throw new ArgumentException("Empty ConnectUserQuery supplied");
            }

            var url = GetConnectUrl("user") + "?" + dict.GetQueryString();

            var request = new HttpRequest
            {
                Method = "GET",
                Url = url,
                CancellationToken = cancellationToken
            };

            AddUserAccessToken(request, accessToken);
            AddAppInfo(request, _appName, _appVersion);

            using (var stream = await _httpClient.SendAsync(request).ConfigureAwait(false))
            {
                return JsonSerializer.DeserializeFromStream<ConnectUser>(stream);
            }
        }

        public async Task<ConnectUserServer[]> GetServers(string userId, string accessToken, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException("userId");
            }
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }

            var dict = new QueryStringDictionary();

            dict.Add("userId", userId);

            var url = GetConnectUrl("servers") + "?" + dict.GetQueryString();

            var request = new HttpRequest
            {
                Method = "GET",
                Url = url,
                CancellationToken = cancellationToken
            };

            AddUserAccessToken(request, accessToken);
            AddAppInfo(request, _appName, _appVersion);

            using (var stream = await _httpClient.SendAsync(request).ConfigureAwait(false))
            {
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync().ConfigureAwait(false);

                    _logger.Debug("Connect servers response: {0}", json);

                    return JsonSerializer.DeserializeFromString<ConnectUserServer[]>(json);
                }
            }
        }

        private void AddUserAccessToken(HttpRequest request, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }
            request.RequestHeaders["X-Connect-UserToken"] = accessToken;
        }

        private void AddAppInfo(HttpRequest request, string appName, string appVersion)
        {
            if (string.IsNullOrWhiteSpace(appName))
            {
                throw new ArgumentNullException("appName");
            }
            if (string.IsNullOrWhiteSpace(appVersion))
            {
                throw new ArgumentNullException("appVersion");
            }
            request.RequestHeaders["X-Application"] = appName + "/" + appVersion;
        }

        private string GetConnectUrl(string handler)
        {
            if (string.IsNullOrWhiteSpace(handler))
            {
                throw new ArgumentNullException("handler");
            }
            return "https://connect.emby.media/service/" + handler;
        }

        public async Task<ConnectSignupResponse> SignupForConnect(string email, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("email");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("username");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("password");
            }
            if (password.Length < 8)
            {
                throw new ArgumentException("password must be at least 8 characters");
            }
            var request = new HttpRequest
            {
                Url = GetConnectUrl("register"),
                Method = "POST"
            };

            var dict = new QueryStringDictionary();

            dict.Add("email", Uri.EscapeDataString(email));
            dict.Add("userName", username);
            dict.Add("password", password);
            request.SetPostData(dict);

            request.RequestHeaders["X-Connect-Token"] = "CONNECT-REGISTER";
            AddAppInfo(request, _appName, _appVersion);

            using (var response = await _httpClient.GetResponse(request, true).ConfigureAwait(false))
            {
                var responseObject = JsonSerializer.DeserializeFromStream<RawConnectResponse>(response.Content);

                if (string.Equals(responseObject.Status, "SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    return ConnectSignupResponse.Success;
                }
                if (string.Equals(responseObject.Status, "USERNAME_IN_USE", StringComparison.OrdinalIgnoreCase))
                {
                    return ConnectSignupResponse.UsernameInUser;
                }
                if (string.Equals(responseObject.Status, "EMAIL_IN_USE", StringComparison.OrdinalIgnoreCase))
                {
                    return ConnectSignupResponse.EmailInUse;
                }
                return ConnectSignupResponse.Failure;
            }
        }

        private class RawConnectResponse
        {
            public string Status { get; set; }
            public string Message { get; set; }
        }
    }
}
