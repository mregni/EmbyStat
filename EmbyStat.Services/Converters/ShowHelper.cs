using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Helpers;
using EmbyStat.Common.Models.Joins;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Services.Converters
{
    public static class ShowHelper
    {
        public static Show ConvertToShow(BaseItemDto show)
        {
            return new Show
            {
                Id = show.Id,
                Primary = show.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value,
                Thumb = show.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value,
                Logo = show.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value,
                Banner = show.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value,
                Name = show.Name,
                ParentId = show.ParentId,
                Path = show.Path,
                CommunityRating = show.CommunityRating,
                CumulativeRunTimeTicks = show.CumulativeRunTimeTicks,
                DateCreated = show.DateCreated,
                DateLastMediaAdded = show.DateLastMediaAdded,
                HomePageUrl = show.HomePageUrl,
                IMDB = show.ProviderIds.FirstOrDefault(y => y.Key == "Imdb").Value,
                TMDB = show.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value,
                TVDB = show.ProviderIds.FirstOrDefault(y => y.Key == "Tvdb").Value,
                OfficialRating = show.OfficialRating,
                Overview = show.Overview,
                PremiereDate = show.PremiereDate,
                ProductionYear = show.ProductionYear,
                RunTimeTicks = show.RunTimeTicks,
                SortName = show.SortName,
                Status = show.Status,
                MediaGenres = show.GenreItems.Select(y => new MediaGenre()
                {
                    GenreId = y.Id,
                    MediaId = show.Id
                }).ToList(),
                ExtraPersons = show.People.GroupBy(y => y.Id).Select(y => y.First()).Select(y => new ExtraPerson()
                {
                    ExtraId = show.Id,
                    PersonId = y.Id,
                    Type = y.Type
                }).ToList()
            };
        }

        public static Season ConvertToSeason(BaseItemDto season, IEnumerable<Episode> episodes)
        {
            return new Season
            {
                Id = season.Id,
                Name = season.Name,
                ParentId = season.ParentId,
                Path = season.Path,
                DateCreated = season.DateCreated,
                IndexNumber = season.IndexNumber,
                IndexNumberEnd = season.IndexNumberEnd,
                PremiereDate = season.PremiereDate,
                ProductionYear = season.ProductionYear,
                SortName = season.SortName,
                Primary = season.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value,
                Thumb = season.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value,
                Logo = season.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value,
                Banner = season.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value,
                SeasonEpisodes = episodes.Select(x => new SeasonEpisode
                {
                    EpisodeId = x.Id,
                    SeasonId = season.Id
                }).ToList()
            };
        }

        public static Episode ConvertToEpisode(BaseItemDto episode)
        {
            return new Episode
            {
                Id = episode.Id,
                Name = episode.Name,
                Path = episode.Path,
                ParentId = String.Empty,
                CommunityRating = episode.CommunityRating,
                Container = episode.Container,
                DateCreated = episode.DateCreated,
                DvdEpisodeNumber = episode.DvdEpisodeNumber,
                DvdSeasonNumber = episode.DvdSeasonNumber,
                HasSubtitles = episode.HasSubtitles,
                IndexNumber = episode.IndexNumber,
                IndexNumberEnd = episode.IndexNumberEnd,
                MediaType = episode.Type,
                Overview = episode.Overview,
                ProductionYear = episode.ProductionYear,
                PremiereDate = episode.PremiereDate,
                RunTimeTicks = episode.RunTimeTicks,
                SortName = episode.SortName,
                IdHD = episode.IsHD,
                Primary = episode.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value,
                Thumb = episode.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value,
                Logo = episode.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value,
                Banner = episode.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value,
                IMDB = episode.ProviderIds.FirstOrDefault(y => y.Key == "Imdb").Value,
                TMDB = episode.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value,
                TVDB = episode.ProviderIds.FirstOrDefault(y => y.Key == "Tvdb").Value,
                AudioStreams = episode.MediaStreams.Where(y => y.Type == MediaStreamType.Audio).Select(y => new AudioStream()
                {
                    Id = Guid.NewGuid().ToString(),
                    VideoId = episode.Id,
                    BitRate = y.BitRate,
                    ChannelLayout = y.ChannelLayout,
                    Channels = y.Channels,
                    Codec = y.Codec,
                    Language = y.Language,
                    SampleRate = y.SampleRate
                }).ToList(),
                SubtitleStreams = episode.MediaStreams.Where(y => y.Type == MediaStreamType.Subtitle).Select(y => new SubtitleStream()
                {
                    Id = Guid.NewGuid().ToString(),
                    Language = y.Language,
                    Codec = y.Codec,
                    DisplayTitle = y.DisplayTitle,
                    IsDefault = y.IsDefault,
                    VideoId = episode.Id
                }).ToList(),
                VideoStreams = episode.MediaStreams.Where(y => y.Type == MediaStreamType.Video).Select(y => new VideoStream()
                {
                    Id = Guid.NewGuid().ToString(),
                    VideoId = episode.Id,
                    Language = y.Language,
                    BitRate = y.BitRate,
                    AspectRatio = y.AspectRatio,
                    AverageFrameRate = y.AverageFrameRate,
                    Channels = y.Channels,
                    Height = y.Height,
                    Width = y.Width
                }).ToList(),
                MediaSources = episode.MediaSources.Select(y => new MediaSource
                {
                    Id = Guid.NewGuid().ToString(),
                    Path = y.Path,
                    BitRate = y.Bitrate,
                    Container = y.Container,
                    Protocol = y.Protocol.ToString(),
                    RunTimeTicks = y.RunTimeTicks,
                    VideoId = episode.Id,
                    VideoType = y.VideoType.ToString()
                }).ToList()
            };
        }
    }
}
