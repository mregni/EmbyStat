using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Services.Emby.Models;
using EmbyStat.Services.EmbyClient.Cryptography;
using EmbyStat.Services.EmbyClient.Model;
using EmbyStat.Services.EmbyClient.Net;
using EmbyStat.Services.Helpers;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Users;

namespace EmbyStat.Services.EmbyClient
{
	public class EmbyClient : IEmbyClient
	{
		public IDevice Device { get; private set; }
		public string ServerAddress { get; protected set; }
		public string ClientName { get; set; }
		public string DeviceName => Device.DeviceName;
		public string ApplicationVersion { get; set; }
		public string DeviceId => Device.DeviceId;
		public string AccessToken { get; private set; }
		public string CurrentUserId { get; private set; }
		private readonly HttpHeaders _httpHeaders = new HttpHeaders();
		private readonly ICryptographyProvider _cryptographyProvider;
		private readonly IJsonSerializer _jsonSerializer;
		private readonly IAsyncHttpClient _httpClient;

		public EmbyClient(ICryptographyProvider cryptographyProvider, IJsonSerializer jsonSerializer, IAsyncHttpClient httpClient)
		{
			_cryptographyProvider = cryptographyProvider;
			_jsonSerializer = jsonSerializer;
			_httpClient = httpClient;

			ClientName = Constants.AppName;
			ApplicationVersion = "1.0.0";
			Device = new Device
			{
				DeviceId = Constants.DeviceId,
				DeviceName = Constants.DeviceName
			};

			ResetHttpHeaders();
		}

		private string ApiUrl => ServerAddress + "/emby";
		private string AuthorizationScheme => "MediaBrowser";

		private void ChangeServerLocation(string address, bool keepExistingAuth = false)
		{
			ServerAddress = address;

			if (!keepExistingAuth)
			{
				SetAuthenticationInfo(null, null);
			}
		}

		private void SetAuthenticationInfo(string accessToken, string userId)
		{
			CurrentUserId = userId;
			AccessToken = accessToken;
			ResetHttpHeaders();
		}

		private void ClearAuthenticationInfo()
		{
			CurrentUserId = null;
			AccessToken = null;
			ResetHttpHeaders();
		}

		private void SetAuthenticationInfo(string accessToken)
		{
			CurrentUserId = null;
			AccessToken = accessToken;
			ResetHttpHeaders();
		}

		private void ResetHttpHeaders()
		{
			_httpHeaders.SetAccessToken(AccessToken);

			var authValue = AuthorizationParameter;

			if (string.IsNullOrEmpty(authValue))
			{
				ClearHttpRequestHeader("Authorization");
				SetAuthorizationHttpRequestHeader(null, null);
			}
			else
			{
				SetAuthorizationHttpRequestHeader(AuthorizationScheme, authValue);
			}
		}

		private void SetAuthorizationHttpRequestHeader(string scheme, string parameter)
		{
			_httpHeaders.AuthorizationScheme = scheme;
			_httpHeaders.AuthorizationParameter = parameter;
		}

		private void ClearHttpRequestHeader(string name)
		{
			_httpHeaders.Remove(name);
		}

		private string AuthorizationParameter
		{
			get
			{
				if (string.IsNullOrEmpty(ClientName) && string.IsNullOrEmpty(DeviceId) && string.IsNullOrEmpty(DeviceName))
				{
					return string.Empty;
				}

				var header = $"Client=\"{ClientName}\", DeviceId=\"{DeviceId}\", Device=\"{DeviceName}\", Version=\"{ApplicationVersion}\"";

				if (!string.IsNullOrEmpty(CurrentUserId))
				{
					header += string.Format(", UserId=\"{0}\"", CurrentUserId);
				}

				return header;
			}
		}

		private string AddDataFormat(string url)
		{
			const string format = "json";

			if (url.IndexOf('?') == -1)
			{
				url += "?format=" + format;
			}
			else
			{
				url += "&format=" + format;
			}

			return url;
		}

		public async Task<AuthenticationResult> AuthenticateUserAsync(string username, string password, string address)
		{
			if (string.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentNullException(nameof(username));
			}

			if (string.IsNullOrWhiteSpace(address))
			{
				throw new ArgumentNullException(nameof(address));
			}

			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentNullException(nameof(password));
			}

			ServerAddress = address;
			var bytes = Encoding.UTF8.GetBytes(password);
			var args = new Dictionary<string, string>
			{
				["username"] = Uri.EscapeDataString(username),
				["pw"] = password,
				["password"] = BitConverter.ToString(_cryptographyProvider.CreateSha1(bytes)).Replace("-", string.Empty),
				["passwordMD5"] = GetConnectPasswordMd5(password)
			};

			var url = GetApiUrl("Users/AuthenticateByName");
			var result = await PostAsync<AuthenticationResult>(url, args, CancellationToken.None);

			SetAuthenticationInfo(result.AccessToken, result.User.Id);

			return result;
		}

		public async Task<List<PluginInfo>> GetInstalledPluginsAsync()
		{
			var url = GetApiUrl("Plugins");

			using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
			{
				return DeserializeFromStream<List<PluginInfo>>(stream);
			}
		}

		private Task<Stream> GetSerializedStreamAsync(string url, CancellationToken cancellationToken)
		{
			url = AddDataFormat(url);

			return GetStream(url, cancellationToken);
		}

		private Task<Stream> GetSerializedStreamAsync(string url)
		{
			return GetSerializedStreamAsync(url, CancellationToken.None);
		}

		private string GetConnectPasswordMd5(string password)
		{
			var bytes = Encoding.UTF8.GetBytes(password);
			bytes = _cryptographyProvider.CreateMD5(bytes);

			var hash = BitConverter.ToString(bytes, 0, bytes.Length).Replace("-", string.Empty);
			return hash;
		}

		private string GetApiUrl(string handler)
		{
			return GetApiUrl(handler, new QueryStringDictionary());
		}

		private string GetApiUrl(string handler, QueryStringDictionary queryString)
		{
			if (string.IsNullOrEmpty(handler))
			{
				throw new ArgumentNullException(nameof(handler));
			}

			if (queryString == null)
			{
				throw new ArgumentNullException(nameof(queryString));
			}

			return queryString.GetUrl(ApiUrl + "/" + handler);
		}

		private async Task<T> PostAsync<T>(string url, Dictionary<string, string> args, CancellationToken cancellationToken = default(CancellationToken)) where T : class
		{
			url = AddDataFormat(url);

			var strings = args.Keys.Select(key => string.Format("{0}={1}", key, args[key]));
			var postContent = string.Join("&", strings.ToArray());

			const string contentType = "application/x-www-form-urlencoded";

			using (var stream = await SendAsync(new HttpRequest
			{
				Url = url,
				CancellationToken = cancellationToken,
				RequestHeaders = _httpHeaders,
				Method = "POST",
				RequestContentType = contentType,
				RequestContent = postContent
			}).ConfigureAwait(false))
			{
				return DeserializeFromStream<T>(stream);
			}
		}

		private Task<Stream> GetStream(string url, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync(new HttpRequest
			{
				CancellationToken = cancellationToken,
				Method = "GET",
				RequestHeaders = _httpHeaders,
				Url = url
			});
		}

		private T DeserializeFromStream<T>(Stream stream)
			where T : class
		{
			return (T)DeserializeFromStream(stream, typeof(T));
		}

		private object DeserializeFromStream(Stream stream, Type type)
		{
			return _jsonSerializer.DeserializeFromStream(stream, type);
		}

		private async Task<Stream> SendAsync(HttpRequest request)
		{
			return await _httpClient.SendAsync(request).ConfigureAwait(false);
		}

		public void Dispose()
		{
		}
	}
}
