using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.EmbyClient.Net;
using EmbyStat.Clients.Tvdb.Converter;
using EmbyStat.Clients.Tvdb.Models;
using EmbyStat.Common;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Entities;
using NLog;

namespace EmbyStat.Clients.Tvdb
{
    public class TvdbClient : ITvdbClient
    {
        private TvdbToken JWtoken;
        private readonly IAsyncHttpClient _httpClient;
        private readonly Logger _logger;

        public TvdbClient(IAsyncHttpClient httpClient)
        {
            _httpClient = httpClient;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task Login(string apiKey, CancellationToken cancellationToken)
        {
            _logger.Info($"{Constants.LogPrefix.TheTVDBCLient}\tLogging in on theTVDB API with key: {apiKey}");
            var httpRequest = new HttpRequest
            {
                CancellationToken = cancellationToken,
                Method = "POST",
                Url = $"{Constants.Tvdb.BaseUrl}{Constants.Tvdb.LoginUrl}",
                RequestContent = "{ \"apikey\": \""+ apiKey + "\"}",
                RequestContentType = "application/json"
            };

            using (var stream = await _httpClient.SendAsync(httpRequest))
            {
                JWtoken = JsonSerializerExtentions.DeserializeFromStream<TvdbToken>(stream);
            }
        }

        public async Task<IEnumerable<VirtualEpisode>> GetEpisodes(string seriesId, CancellationToken cancellationToken)
        {
            var tvdbEpisodes = new List<VirtualEpisode>();
            TvdbEpisodes page;
            var i = 0;
            do
            {
                i++;
                var url = string.Format(Constants.Tvdb.SerieEpisodesUrl, seriesId, i);
                page = await GetEpisodePage(url, cancellationToken);
                tvdbEpisodes.AddRange(page.Data
                    .Where(x => x.AiredSeason != 0 && !string.IsNullOrWhiteSpace(x.FirstAired) && DateTime.Now.Date >= Convert.ToDateTime(x.FirstAired)).Select(EpisodeHelper.ConvertToEpisode));
            } while (page.Links.Next != i && page.Links.Next != null);

            return tvdbEpisodes;
        }

        private async Task<TvdbEpisodes> GetEpisodePage(string url, CancellationToken cancellationToken)
        {
            _logger.Info($"{Constants.LogPrefix.TheTVDBCLient}\tCall to THETVDB: {Constants.Tvdb.BaseUrl}{url}");
            var headers = new HttpHeaders {{"Authorization", $"Bearer {JWtoken.Token}"}};
            var httpRequest = new HttpRequest
            {
                CancellationToken = cancellationToken,
                Method = "GET",
                Url = $"{Constants.Tvdb.BaseUrl}{url}",
                RequestHeaders = headers
            };

            using (var stream = await _httpClient.SendAsync(httpRequest))
            {
                return JsonSerializerExtentions.DeserializeFromStream<TvdbEpisodes>(stream);
            }
        }

        public async Task<IEnumerable<string>> GetShowsToUpdate(IEnumerable<string> showIds, DateTime lastUpdateTime, CancellationToken cancellationToken)
        {
            _logger.Info($"{Constants.LogPrefix.TheTVDBCLient}\tCalling TheTVDB to udpated shows");
            try
            {
                var updateList = new List<string>();
                for (var i = lastUpdateTime; i < DateTime.Now; i = i.AddDays(7))
                {
                    var offset = new DateTimeOffset(i);
                    var epochTimeFrom = offset.ToUnixTimeSeconds();
                    var epochTimeTo = offset.AddDays(7).ToUnixTimeSeconds();

                    var url = string.Format(Constants.Tvdb.UpdatesUrl, epochTimeFrom, epochTimeTo);
                    var httpRequest = new HttpRequest
                    {
                        CancellationToken = cancellationToken,
                        Method = "GET",
                        Url = $"{Constants.Tvdb.BaseUrl}{url}"
                    };

                    _logger.Info($"{Constants.LogPrefix.TheTVDBCLient}\tCall to THETVDB: {Constants.Tvdb.BaseUrl}{url}");
                    using (var stream = await _httpClient.SendAsync(httpRequest))
                    {
                        var list = JsonSerializerExtentions.DeserializeFromStream<Updates>(stream);
                        var neededList = list.Data.Where(x => showIds.Any(y => y == x.Id.ToString())).Select(x => x.Id.ToString()).ToList();
                        updateList.AddRange(neededList);
                    }
                }

                return updateList;
            }
            catch (Exception e)
            {
                _logger.Error(e, $"{Constants.LogPrefix.TheTVDBCLient}Could not receive show list from TVDB");
                return new List<string>();
            }
        }
    }
}
