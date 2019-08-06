using System;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Common.Converters
{
    public static class ShowConverter
    {
        public static Show ConvertToShow(BaseItemDto show, string collectionId)
        {
            return new Show
            {
                Id = Convert.ToInt32(show.Id),
                CollectionId = collectionId,
                Primary = show.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value,
                Thumb = show.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value,
                Logo = show.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value,
                Banner = show.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value,
                Name = show.Name,
                ParentId = show.ParentId,
                Path = show.Path,
                CommunityRating = show.CommunityRating,
                DateCreated = show.DateCreated,
                IMDB = show.ProviderIds.FirstOrDefault(y => y.Key == "Imdb").Value,
                TMDB = show.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value,
                TVDB = show.ProviderIds.FirstOrDefault(y => y.Key == "Tvdb").Value,
                OfficialRating = show.OfficialRating,
                PremiereDate = show.PremiereDate,
                ProductionYear = show.ProductionYear,
                RunTimeTicks = show.RunTimeTicks,
                SortName = show.SortName,
                Status = show.Status,
                Genres = show.Genres,
                People = show.People
                    .Where(x => x.Name.Length > 1)
                    .GroupBy(y => y.Id)
                    .Select(y => y.First())
                    .Select(y => new ExtraPerson
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Type = y.Type
                    }).ToArray()
            };
        }

        public static Season ConvertToSeason(BaseItemDto season)
        {
            return new Season
            {
                Id = Convert.ToInt32(season.Id),
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
                Banner = season.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value
            };
        }

        public static Episode ConvertToEpisode(BaseItemDto episode, Show show)
        {
            return new Episode
            {
                Id = Convert.ToInt32(episode.Id),
                ShowId = Convert.ToInt32(show.Id),
                Name = episode.Name,
                Path = episode.Path,
                ParentId = episode.ParentId,
                CommunityRating = episode.CommunityRating,
                Container = episode.Container,
                DateCreated = episode.DateCreated,
                IndexNumber = episode.IndexNumber,
                IndexNumberEnd = episode.IndexNumberEnd,
                MediaType = episode.Type,
                ProductionYear = episode.ProductionYear,
                PremiereDate = episode.PremiereDate,
                RunTimeTicks = episode.RunTimeTicks,
                SortName = episode.SortName,
                ShowName = episode.SeriesName,
                Primary = episode.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value,
                Thumb = episode.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value,
                Logo = episode.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value,
                Banner = episode.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value,
                IMDB = episode.ProviderIds.FirstOrDefault(y => y.Key == "Imdb").Value,
                TMDB = episode.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value,
                TVDB = episode.ProviderIds.FirstOrDefault(y => y.Key == "Tvdb").Value,
                AudioStreams = episode.MediaStreams.Where(y => y.Type == MediaStreamType.Audio).Select(y => new AudioStream
                {
                    Id = Guid.NewGuid().ToString(),
                    BitRate = y.BitRate,
                    ChannelLayout = y.ChannelLayout,
                    Channels = y.Channels,
                    Codec = y.Codec,
                    Language = y.Language,
                    SampleRate = y.SampleRate
                }).ToList(),
                SubtitleStreams = episode.MediaStreams.Where(y => y.Type == MediaStreamType.Subtitle).Select(y => new SubtitleStream
                {
                    Id = Guid.NewGuid().ToString(),
                    Language = y.Language,
                    Codec = y.Codec,
                    DisplayTitle = y.DisplayTitle,
                    IsDefault = y.IsDefault
                }).ToList(),
                VideoStreams = episode.MediaStreams.Where(y => y.Type == MediaStreamType.Video).Select(y => new VideoStream
                {
                    Id = Guid.NewGuid().ToString(),
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
                    RunTimeTicks = y.RunTimeTicks
                }).ToList()
            };
        }
    }
}
