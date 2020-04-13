using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using EmbyStat.Clients.Tvdb.Converter;
using EmbyStat.Clients.Tvdb.Models;
using EmbyStat.Common;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Show;
using MediaBrowser.Model.Net;
using NLog;
using RestSharp;

namespace EmbyStat.Clients.Tvdb
{
    public class TvdbClient : ITvdbClient
    {
        private string _jwToken;
        private readonly Logger _logger;
        private readonly IRestClient _restClient;

        public TvdbClient(IRestClient client)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _restClient = client.Initialize();
            _restClient.BaseUrl = new Uri(Constants.Tvdb.BaseUrl);
        }

        public bool Login(string apiKey)
        {
            _logger.Info($"{Constants.LogPrefix.TheTVDBCLient}\tLogging in on theTVDB API with key: {apiKey}");

            try
            {
                var request = new RestRequest(Constants.Tvdb.LoginUrl, Method.POST);
                request.AddJsonBody(new { apikey = apiKey });

                var result = _restClient.Execute<TvdbToken>(request);
                _jwToken = result.Data.Token;

                return true;
            }
            catch (HttpException e)
            {
                if (e.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new BusinessException("TVDB_LOGIN_FAILED", 500, e);
                }

                throw;
            }
        }

        public IEnumerable<VirtualEpisode> GetEpisodes(string seriesId)
        {
            var tvdbEpisodes = new List<VirtualEpisode>();
            TvdbEpisodes page;
            var i = 0;
            do
            {
                i++;
                var url = string.Format(Constants.Tvdb.SerieEpisodesUrl, seriesId, i);
                page = GetEpisodePage(url);
                page.Data
                    .ForEach(x =>
                    {
                        if (string.IsNullOrWhiteSpace(x.FirstAired) || !DateTime.TryParse(x.FirstAired, out _))
                        {
                            x.FirstAired = DateTime.MaxValue.ToString("O");
                        }
                    });
                tvdbEpisodes.AddRange(page.Data
                    .Where(x => x.AiredSeason != 0 && !string.IsNullOrWhiteSpace(x.FirstAired) && DateTime.Now.Date >= Convert.ToDateTime(x.FirstAired, CultureInfo.InvariantCulture))
                    .Select(x => x.ConvertToVirtualEpisode()));
            } while (page.Links.Next != i && page.Links.Next != null);

            return tvdbEpisodes;
        }

        private TvdbEpisodes GetEpisodePage(string url)
        {
            _logger.Info($"{Constants.LogPrefix.TheTVDBCLient}\tCall to THETVDB: {Constants.Tvdb.BaseUrl}{url}");

            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {_jwToken}");

            var result = _restClient.Execute<TvdbEpisodes>(request);

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                throw new HttpException("404 Not Found");
            }

            return result.Data;
        }

        public List<string> GetShowsToUpdate(DateTime? lastUpdateTime)
        {
            if (!lastUpdateTime.HasValue)
            {
                return new List<string>();
            }

            _logger.Info($"{Constants.LogPrefix.TheTVDBCLient}\tCalling TheTVDB for updated shows");
            try
            {
                var updateList = new List<string>();
                for (var i = lastUpdateTime.Value; i < DateTime.Now; i = i.AddDays(7))
                {
                    var offset = new DateTimeOffset(i);
                    var epochTimeFrom = offset.ToUnixTimeSeconds();
                    var epochTimeTo = offset.AddDays(7).ToUnixTimeSeconds();

                    var url = string.Format(Constants.Tvdb.UpdatesUrl, epochTimeFrom, epochTimeTo);

                    var request = new RestRequest(url, Method.GET);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", $"Bearer {_jwToken}");

                    var result = _restClient.Execute<Updates>(request);
                    updateList.AddRange(result.Data.Data.Select(x => x.Id.ToString()));

                    _logger.Info($"{Constants.LogPrefix.TheTVDBCLient}\tCall to THETVDB: {Constants.Tvdb.BaseUrl}{url}");
                }

                return updateList;
            }
            catch (Exception e)
            {
                _logger.Error(e, $"{Constants.LogPrefix.TheTVDBCLient} Could not receive show list from TVDB");
                return new List<string>();
            }
        }
    }
}
