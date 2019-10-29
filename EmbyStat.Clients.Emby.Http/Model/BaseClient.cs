using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EmbyStat.Clients.Emby.Http.Model;
using EmbyStat.Common;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Net;
using NLog;

namespace EmbyStat.Clients.Emby.Http
{
	public abstract class BaseClient
	{
		protected readonly HttpHeaders HttpHeaders = new HttpHeaders();
		protected readonly IAsyncHttpClient HttpClient;
        protected readonly Logger Logger;

		protected BaseClient(IAsyncHttpClient httpClient)
		{
			HttpClient = httpClient;
            
		}

		public void SetAddressAndUser(string url, string token, string userId)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				throw new ArgumentNullException(nameof(url));
			}

			if (string.IsNullOrWhiteSpace(token))
			{
				throw new ArgumentNullException(nameof(token));
			}

   //         ServerAddress = url;
			//AccessToken = token;
   //         CurrentUserId = new Guid(userId);
			//ResetHttpHeaders();
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

        protected string GetApiUrl(string handler, QueryStringDictionary queryString = null)
        {
            return GetApiUrl(string.Empty, handler, queryString ?? new QueryStringDictionary());
        }

        protected string GetApiUrl(string apiUrl, string handler)
        {
            return GetApiUrl(apiUrl, handler, new QueryStringDictionary());
        }

		protected string GetApiUrl(string apiUrl, string handler, QueryStringDictionary queryString)
		{
			if (string.IsNullOrEmpty(handler))
			{
				throw new ArgumentNullException(nameof(handler));
			}

			if (queryString == null)
			{
				throw new ArgumentNullException(nameof(queryString));
			}

			return queryString.GetUrl(apiUrl + "/" + handler);
		}

		protected async Task<T> PostAsync<T>(string url, Dictionary<string, string> args, CancellationToken cancellationToken = default(CancellationToken)) where T : class
		{
			url = AddDataFormat(url);

			var strings = args.Keys.Select(key => $"{key}={args[key]}");
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
				}))
				{
					return DeserializeFromStream<T>(stream);
				}
			}
			catch (Exception e)
			{
                Logger.Error(e, $"{Constants.LogPrefix.EmbyClient}\tCall to Emby failed");
				throw new BusinessException("EMBY_CALL_FAILED", 500, e);
			}

		}

		protected async Task<string> PostAsyncToString(string url, Dictionary<string, string> args, int timeout, CancellationToken cancellationToken = default(CancellationToken))
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
                    Timeout = timeout,
					CancellationToken = cancellationToken,
					RequestHeaders = HttpHeaders,
					Method = "POST",
					RequestContentType = contentType,
					RequestContent = postContent
				}))
				{
					var reader = new StreamReader(stream);
					return reader.ReadToEnd();
				}
			}
			catch (Exception e)
			{
                Logger.Error(e, $"{Constants.LogPrefix.EmbyClient}\tCall to Emby failed");
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
                Logger.Error(e, $"{Constants.LogPrefix.EmbyClient}\tCall to Emby failed");
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
			return JsonSerializerExtentions.DeserializeFromStream(stream, type);
		}

		protected async Task<Stream> SendAsync(HttpRequest request)
		{
            Logger.Info($"{Constants.LogPrefix.EmbyClient}\tSending {request.Method.ToUpper()}: {request.Url}");
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

            var encodedUrl = HttpUtility.UrlEncode(url);
            return GetApiUrl(encodedUrl, dict);
        }

	    //protected string GetItemByNameListUrl(string type, ItemQuery query)
	    //{
	    //    if (query == null)
	    //    {
	    //        throw new ArgumentNullException("query");
	    //    }

	    //    var dict = new QueryStringDictionary { };

	    //    if (query.SortOrder.HasValue)
	    //    {
	    //        dict["sortOrder"] = query.SortOrder.ToString();
	    //    }

	    //    if (query.Fields != null)
	    //    {
	    //        dict.Add("fields", query.Fields.Select(f => f.ToString()));
	    //    }

	    //    if (query.Filters != null)
	    //    {
	    //        dict.Add("Filters", query.Filters.Select(f => f.ToString()));
	    //    }

	    //    if (query.ImageTypes != null)
	    //    {
	    //        dict.Add("ImageTypes", query.ImageTypes.Select(f => f.ToString()));
	    //    }

	    //    if (query.EnableImageTypes != null)
	    //    {
	    //        dict.Add("EnableImageTypes", query.EnableImageTypes.Select(f => f.ToString()));
	    //    }

	    //    dict.AddIfNotNull("IsPlayed", query.IsPlayed);
     //       dict.AddIfNotNullOrEmpty("ParentId", query.ParentId);
	    //    dict.Add("UserId", query.UserId);
	    //    dict.AddIfNotNull("StartIndex", query.StartIndex);
	    //    dict.AddIfNotNull("Limit", query.Limit);
	    //    dict.AddIfNotNull("SortBy", query.SortBy);
	    //    dict.Add("recursive", query.Recursive);
	    //    dict.AddIfNotNull("MediaTypes", query.MediaTypes);
	    //    dict.AddIfNotNull("ExcludeItemTypes", query.ExcludeItemTypes);
	    //    dict.AddIfNotNull("IncludeItemTypes", query.IncludeItemTypes);
	    //    dict.AddIfNotNullOrEmpty("NameLessThan", query.NameLessThan);
	    //    dict.AddIfNotNullOrEmpty("NameStartsWithOrGreater", query.NameStartsWithOrGreater);
	    //    dict.AddIfNotNull("EnableImages", query.EnableImages);
     //       dict.AddIfNotNull("ImageTypeLimit", query.ImageTypeLimit);

	    //    return GetApiUrl(type, dict);
	    //}
    }
}
