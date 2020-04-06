using System;
using System.Linq;
using EmbyStat.Clients.Base.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Clients.Base.Converters
{
    public static class MovieConverter
    {
        public static Movie ConvertToMovie(this BaseItemDto x, string collectionId)
        {
            return new Movie
            {
                Id = x.Id,
                CollectionId = collectionId,
                Name = x.Name,
                ParentId = x.ParentId,
                OriginalTitle = x.OriginalTitle,
                DateCreated = x.DateCreated,
                Path = x.Path,
                SortName = x.SortName,
                MediaSources = x.MediaSources.Select(y => new MediaSource
                {
                    Id = y.Id,
                    Path = y.Path,
                    BitRate = y.Bitrate,
                    Container = y.Container,
                    Protocol = y.Protocol.ToString(),
                    RunTimeTicks = y.RunTimeTicks,
                    SizeInMb = Math.Round(y.Size / (double)1024 / 1024 ?? 0, MidpointRounding.AwayFromZero)
                }).ToList(),
                RunTimeTicks = x.RunTimeTicks,
                Container = x.Container,
                CommunityRating = x.CommunityRating,
                MediaType = x.MediaType,
                OfficialRating = x.OfficialRating,
                PremiereDate = x.PremiereDate,
                ProductionYear = x.ProductionYear,
                Video3DFormat = x.Video3DFormat ?? 0,
                Primary = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value,
                Thumb = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value,
                Logo = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value,
                Banner = x.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value,
                IMDB = x.ProviderIds.FirstOrDefault(y => y.Key == "Imdb").Value,
                TMDB = x.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value,
                TVDB = x.ProviderIds.FirstOrDefault(y => y.Key == "Tvdb").Value,
                AudioStreams = x.MediaStreams.Where(y => y.Type == MediaStreamType.Audio).Select(y => new AudioStream
                {
                    Id = Guid.NewGuid().ToString(),
                    BitRate = y.BitRate,
                    ChannelLayout = y.ChannelLayout,
                    Channels = y.Channels,
                    Codec = y.Codec,
                    Language = y.Language,
                    SampleRate = y.SampleRate
                }).ToList(),
                SubtitleStreams = x.MediaStreams.Where(y => y.Type == MediaStreamType.Subtitle).Select(y => new SubtitleStream
                {
                    Id = Guid.NewGuid().ToString(),
                    Language = y.Language,
                    Codec = y.Codec,
                    DisplayTitle = y.DisplayTitle,
                    IsDefault = y.IsDefault
                }).ToList(),
                VideoStreams = x.MediaStreams.Where(y => y.Type == MediaStreamType.Video).Select(y => new VideoStream
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
                Genres = x.Genres,
                People = x.People
                    .Where(y => y.Name.Length > 1)
                    .Select(y => new ExtraPerson
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Type = y.Type
                    }).ToArray()
            };
        }
    }
}
