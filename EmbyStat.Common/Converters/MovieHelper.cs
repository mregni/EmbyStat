using System;
using System.Linq;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Joins;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Common.Converters
{
    public static class MovieHelper
    {
        public static Movie ConvertToMovie(BaseItemDto x)
        {
            return new Movie
            {
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId,
                //HomePageUrl = x.HomePageUrl,
                OriginalTitle = x.OriginalTitle,
                DateCreated = x.DateCreated,
                Path = x.Path,
                SortName = x.SortName,
                Overview = x.Overview,
                MediaSources = x.MediaSources.Select(y => new MediaSource
                {
                    Id = y.Id,
                    Path = y.Path,
                    BitRate = y.Bitrate,
                    Container = y.Container,
                    Protocol = y.Protocol.ToString(),
                    RunTimeTicks = y.RunTimeTicks,
                    VideoId = x.Id,
                    VideoType = y.VideoType.ToString()
                }).ToList(),
                RunTimeTicks = x.RunTimeTicks,
                Container = x.Container,
                CommunityRating = x.CommunityRating,
                HasSubtitles = x.HasSubtitles,
                MediaType = x.MediaType,
                OfficialRating = x.OfficialRating,
                PremiereDate = x.PremiereDate,
                ProductionYear = x.ProductionYear,
                IdHD = x.IsHD,
                Primary = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value,
                Thumb = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value,
                Logo = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value,
                Banner = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value,
                IMDB = x.ProviderIds.FirstOrDefault(y => y.Key == "Imdb").Value,
                TMDB = x.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value,
                TVDB = x.ProviderIds.FirstOrDefault(y => y.Key == "Tvdb").Value,
                AudioStreams = x.MediaStreams.Where(y => y.Type == MediaStreamType.Audio).Select(y => new AudioStream()
                {
                    Id = Guid.NewGuid().ToString(),
                    VideoId = x.Id,
                    BitRate = y.BitRate,
                    ChannelLayout = y.ChannelLayout,
                    Channels = y.Channels,
                    Codec = y.Codec,
                    Language = y.Language,
                    SampleRate = y.SampleRate
                }).ToList(),
                SubtitleStreams = x.MediaStreams.Where(y => y.Type == MediaStreamType.Subtitle).Select(y => new SubtitleStream()
                {
                    Id = Guid.NewGuid().ToString(),
                    Language = y.Language,
                    Codec = y.Codec,
                    DisplayTitle = y.DisplayTitle,
                    IsDefault = y.IsDefault,
                    VideoId = x.Id
                }).ToList(),
                VideoStreams = x.MediaStreams.Where(y => y.Type == MediaStreamType.Video).Select(y => new VideoStream()
                {
                    Id = Guid.NewGuid().ToString(),
                    VideoId = x.Id,
                    Language = y.Language,
                    BitRate = y.BitRate,
                    AspectRatio = y.AspectRatio,
                    AverageFrameRate = y.AverageFrameRate,
                    Channels = y.Channels,
                    Height = y.Height,
                    Width = y.Width
                }).ToList(),
                MediaGenres = x.GenreItems.Select(y => new MediaGenre()
                {
                    GenreId = y.Id,
                    MediaId = x.Id
                }).ToList(),
                ExtraPersons = x.People.GroupBy(y => y.Id).Select(y => y.First()).Select(y => new ExtraPerson()
                {
                    ExtraId = x.Id,
                    PersonId = new Guid(y.Id),
                    Type = y.Type
                }).ToList()
            };
        }
    }
}
