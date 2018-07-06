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
using EmbyStat.Common;
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
		protected string AuthorizationScheme => Constants.Emby.AuthorizationScheme;
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

			ClientName = Constants.Emby.AppName;
			ApplicationVersion = "1.0.0";
			Device = new Device
			{
				DeviceId = Constants.Emby.DeviceId,
				DeviceName = Constants.Emby.DeviceName
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

		protected async Task<string> PostAsyncToString(string url, Dictionary<string, string> args, CancellationToken cancellationToken = default(CancellationToken))
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
					var reader = new StreamReader(stream);
					return reader.ReadToEnd();
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
			return await HttpClient.SendAsync(request);
		}

        protected string GetItemListUrl(string url, ItemQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            if (!query.EnableTotalRecordCount)
            {
                dict.Add("EnableTotalRecordCount", query.EnableTotalRecordCount);
            }

            if (query.SortOrder.HasValue)
            {
                dict["sortOrder"] = query.SortOrder.ToString();
            }

            if (query.SeriesStatuses != null)
            {
                dict.Add("SeriesStatuses", query.SeriesStatuses.Select(f => f.ToString()));
            }

            if (query.Fields != null)
            {
                dict.Add("fields", query.Fields.Select(f => f.ToString()));
            }

            if (query.Filters != null)
            {
                dict.Add("Filters", query.Filters.Select(f => f.ToString()));
            }

            if (query.ImageTypes != null)
            {
                dict.Add("ImageTypes", query.ImageTypes.Select(f => f.ToString()));
            }

            if (query.VideoTypes != null)
            {
                dict.Add("VideoTypes", query.VideoTypes.Select(f => f.ToString()));
            }

            if (query.AirDays != null)
            {
                dict.Add("AirDays", query.AirDays.Select(f => f.ToString()));
            }

            if (query.EnableImageTypes != null)
            {
                dict.Add("EnableImageTypes", query.EnableImageTypes.Select(f => f.ToString()));
            }

            if (query.LocationTypes != null && query.LocationTypes.Length > 0)
            {
                dict.Add("LocationTypes", query.LocationTypes.Select(f => f.ToString()));
            }

            if (query.ExcludeLocationTypes != null && query.ExcludeLocationTypes.Length > 0)
            {
                dict.Add("ExcludeLocationTypes", query.ExcludeLocationTypes.Select(f => f.ToString()));
            }

            dict.AddIfNotNullOrEmpty("ParentId", query.ParentId);
            dict.AddIfNotNull("StartIndex", query.StartIndex);
            dict.AddIfNotNull("Limit", query.Limit);
            dict.AddIfNotNull("SortBy", query.SortBy);
            dict.AddIfNotNull("Is3D", query.Is3D);
            dict.AddIfNotNullOrEmpty("MinOfficialRating", query.MinOfficialRating);
            dict.AddIfNotNullOrEmpty("MaxOfficialRating", query.MaxOfficialRating);
            dict.Add("recursive", query.Recursive);
            dict.AddIfNotNull("MinIndexNumber", query.MinIndexNumber);
            dict.AddIfNotNull("EnableImages", query.EnableImages);
            dict.AddIfNotNull("ImageTypeLimit", query.ImageTypeLimit);
            dict.AddIfNotNull("CollapseBoxSetItems", query.CollapseBoxSetItems);
            dict.AddIfNotNull("MediaTypes", query.MediaTypes);
            dict.AddIfNotNull("Genres", query.Genres, "|");
            dict.AddIfNotNull("Ids", query.Ids);
            dict.AddIfNotNull("StudioIds", query.StudioIds, "|");
            dict.AddIfNotNull("ExcludeItemTypes", query.ExcludeItemTypes);
            dict.AddIfNotNull("IncludeItemTypes", query.IncludeItemTypes);
            dict.AddIfNotNull("ArtistIds", query.ArtistIds);
            dict.AddIfNotNull("IsPlayed", query.IsPlayed);
            dict.AddIfNotNull("IsInBoxSet", query.IsInBoxSet);
            dict.AddIfNotNull("PersonIds", query.PersonIds);
            dict.AddIfNotNull("PersonTypes", query.PersonTypes);
            dict.AddIfNotNull("Years", query.Years);
            dict.AddIfNotNull("ParentIndexNumber", query.ParentIndexNumber);
            dict.AddIfNotNull("IsHD", query.IsHD);
            dict.AddIfNotNull("HasParentalRating", query.HasParentalRating);
            dict.AddIfNotNullOrEmpty("SearchTerm", query.SearchTerm);
            dict.AddIfNotNull("MinCriticRating", query.MinCriticRating);
            dict.AddIfNotNull("MinCommunityRating", query.MinCommunityRating);
            dict.AddIfNotNull("MinPlayers", query.MinPlayers);
            dict.AddIfNotNull("MaxPlayers", query.MaxPlayers);
            dict.AddIfNotNullOrEmpty("NameStartsWithOrGreater", query.NameStartsWithOrGreater);
            dict.AddIfNotNullOrEmpty("AlbumArtistStartsWithOrGreater", query.AlbumArtistStartsWithOrGreater);
            dict.AddIfNotNull("IsMissing", query.IsMissing);
            dict.AddIfNotNull("IsUnaired", query.IsUnaired);
            dict.AddIfNotNull("IsVirtualUnaired", query.IsVirtualUnaired);
            dict.AddIfNotNull("AiredDuringSeason", query.AiredDuringSeason);

            return GetApiUrl(url, dict);
        }

	    protected string GetItemByNameListUrl(string type, ItemsByNameQuery query)
	    {
	        if (query == null)
	        {
	            throw new ArgumentNullException("query");
	        }

	        var dict = new QueryStringDictionary { };

	        if (query.SortOrder.HasValue)
	        {
	            dict["sortOrder"] = query.SortOrder.ToString();
	        }

	        if (query.Fields != null)
	        {
	            dict.Add("fields", query.Fields.Select(f => f.ToString()));
	        }

	        if (query.Filters != null)
	        {
	            dict.Add("Filters", query.Filters.Select(f => f.ToString()));
	        }

	        if (query.ImageTypes != null)
	        {
	            dict.Add("ImageTypes", query.ImageTypes.Select(f => f.ToString()));
	        }

	        if (query.EnableImageTypes != null)
	        {
	            dict.Add("EnableImageTypes", query.EnableImageTypes.Select(f => f.ToString()));
	        }

	        dict.AddIfNotNull("IsPlayed", query.IsPlayed);
            dict.AddIfNotNullOrEmpty("ParentId", query.ParentId);
	        dict.Add("UserId", query.UserId);
	        dict.AddIfNotNull("StartIndex", query.StartIndex);
	        dict.AddIfNotNull("Limit", query.Limit);
	        dict.AddIfNotNull("SortBy", query.SortBy);
	        dict.Add("recursive", query.Recursive);
	        dict.AddIfNotNull("MediaTypes", query.MediaTypes);
	        dict.AddIfNotNull("ExcludeItemTypes", query.ExcludeItemTypes);
	        dict.AddIfNotNull("IncludeItemTypes", query.IncludeItemTypes);
	        dict.AddIfNotNullOrEmpty("NameLessThan", query.NameLessThan);
	        dict.AddIfNotNullOrEmpty("NameStartsWithOrGreater", query.NameStartsWithOrGreater);
	        dict.AddIfNotNull("EnableImages", query.EnableImages);
            dict.AddIfNotNull("ImageTypeLimit", query.ImageTypeLimit);

	        return GetApiUrl(type, dict);
	    }
    }
}
