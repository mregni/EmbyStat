using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient.Cryptography;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Api.EmbyClient.Net;
using EmbyStat.Common.Exceptions;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Api.EmbyClient
{
	public abstract class BaseClient<T> where T : class
	{
		protected Device Device { get; }
		protected string ServerAddress { get; set; }
		protected string ClientName { get; set; }
		protected string DeviceName => Device.DeviceName;
		protected string ApplicationVersion { get; set; }
		protected string DeviceId => Device.DeviceId;
		protected string AccessToken { get; private set; }
		protected string CurrentUserId { get; private set; }
		protected string ApiUrl => ServerAddress + "/emby";
		protected string AuthorizationScheme => "MediaBrowser";
		protected readonly HttpHeaders HttpHeaders = new HttpHeaders();
		protected readonly ICryptographyProvider CryptographyProvider;
		protected readonly IJsonSerializer JsonSerializer;
		protected readonly IAsyncHttpClient HttpClient;
		protected readonly ILogger<T> Logger;

		protected BaseClient(ICryptographyProvider cryptographyProvider, IJsonSerializer jsonSerializer, IAsyncHttpClient httpClient, ILogger<T> logger)
		{
			CryptographyProvider = cryptographyProvider;
			JsonSerializer = jsonSerializer;
			HttpClient = httpClient;
			Logger = logger;

			ClientName = Constants.AppName;
			ApplicationVersion = "1.0.0";
			Device = new Device
			{
				DeviceId = Constants.DeviceId,
				DeviceName = Constants.DeviceName
			};

			ResetHttpHeaders();
		}

		public void SetAddressAndUrl(string url, string token)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				throw new ArgumentNullException(nameof(url));
			}

			if (string.IsNullOrWhiteSpace(token))
			{
				throw new ArgumentNullException(nameof(token));
			}

			ServerAddress = url;
			AccessToken = token;
			ResetHttpHeaders();
		}

		protected void ChangeServerLocation(string address, bool keepExistingAuth = false)
		{
			ServerAddress = address;

			if (!keepExistingAuth)
			{
				SetAuthenticationInfo(null, null);
			}
		}

		protected void SetAuthenticationInfo(string accessToken, string userId)
		{
			CurrentUserId = userId;
			AccessToken = accessToken;
			ResetHttpHeaders();
		}

		protected void ClearAuthenticationInfo()
		{
			CurrentUserId = null;
			AccessToken = null;
			ResetHttpHeaders();
		}

		protected void SetAuthenticationInfo(string accessToken)
		{
			CurrentUserId = null;
			AccessToken = accessToken;
			ResetHttpHeaders();
		}

		protected void ResetHttpHeaders()
		{
			HttpHeaders.SetAccessToken(AccessToken);

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

		protected void SetAuthorizationHttpRequestHeader(string scheme, string parameter)
		{
			HttpHeaders.AuthorizationScheme = scheme;
			HttpHeaders.AuthorizationParameter = parameter;
		}

		protected void ClearHttpRequestHeader(string name)
		{
			HttpHeaders.Remove(name);
		}

		protected string AuthorizationParameter
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

		protected string AddDataFormat(string url)
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

		protected Task<Stream> GetSerializedStreamAsync(string url, CancellationToken cancellationToken)
		{
			url = AddDataFormat(url);

			return GetStream(url, cancellationToken);
		}

		protected Task<Stream> GetSerializedStreamAsync(string url)
		{
			return GetSerializedStreamAsync(url, CancellationToken.None);
		}

		protected string GetConnectPasswordMd5(string password)
		{
			var bytes = Encoding.UTF8.GetBytes(password);
			bytes = CryptographyProvider.CreateMD5(bytes);

			var hash = BitConverter.ToString(bytes, 0, bytes.Length).Replace("-", string.Empty);
			return hash;
		}

		protected string GetApiUrl(string handler)
		{
			return GetApiUrl(handler, new QueryStringDictionary());
		}

		protected string GetApiUrl(string handler, QueryStringDictionary queryString)
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

		protected async Task<T> PostAsync<T>(string url, Dictionary<string, string> args, CancellationToken cancellationToken = default(CancellationToken)) where T : class
		{
			url = AddDataFormat(url);

			var strings = args.Keys.Select(key => string.Format("{0}={1}", key, args[key]));
			var postContent = string.Join("&", strings.ToArray());

			const string contentType = "application/x-www-form-urlencoded";

			try
			{
				using (var stream = await SendAsync(new HttpRequest
				{
					Url = url,
					CancellationToken = cancellationToken,
					RequestHeaders = HttpHeaders,
					Method = "POST",
					RequestContentType = contentType,
					RequestContent = postContent
				}).ConfigureAwait(false))
				{
					return DeserializeFromStream<T>(stream);
				}
			}
			catch (Exception e)
			{
				Logger.LogError(e, "EMBY_CALL_FAILED");
				throw new BusinessException("EMBY_CALL_FAILED", 500, e);
			}

		}

		protected Task<Stream> GetStream(string url, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				return SendAsync(new HttpRequest
				{
					CancellationToken = cancellationToken,
					Method = "GET",
					RequestHeaders = HttpHeaders,
					Url = url
				});
			}
			catch (Exception e)
			{
				Logger.LogError(e, "EMBY_CALL_FAILED");
				throw new BusinessException("EMBY_CALL_FAILED", 500, e);
			}
		}

		protected T DeserializeFromStream<T>(Stream stream)
			where T : class
		{
			return (T)DeserializeFromStream(stream, typeof(T));
		}

		protected object DeserializeFromStream(Stream stream, Type type)
		{
			return JsonSerializer.DeserializeFromStream(stream, type);
		}

		protected async Task<Stream> SendAsync(HttpRequest request)
		{
			return await HttpClient.SendAsync(request).ConfigureAwait(false);
		}
	}
}
