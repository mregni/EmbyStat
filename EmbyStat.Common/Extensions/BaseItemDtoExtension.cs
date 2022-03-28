using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Entities.Streams;
using EmbyStat.Common.Models.Net;

namespace EmbyStat.Common.Extensions
{
    public static class BaseItemDtoExtension
    {
        public static T MapStreams<T>(this BaseItemDto dto, T video) where T : Movie
        {
            if (dto.MediaStreams != null)
            {
                video.AudioStreams = dto.MediaStreams
                    .Where(y => y.Type == MediaStreamType.Audio)
                    .Select(y => new AudioStream
                    {
                        BitRate = y.BitRate,
                        ChannelLayout = y.ChannelLayout,
                        Channels = y.Channels,
                        Codec = y.Codec,
                        Language = y.Language,
                        SampleRate = y.SampleRate,
                        IsDefault = y.IsDefault,
                        MovieId = video.Id
                    }).ToList();

                video.SubtitleStreams = dto.MediaStreams
                    .Where(y => y.Type == MediaStreamType.Subtitle)
                    .Select(y => new SubtitleStream
                    {
                        Language = y.Language,
                        Codec = y.Codec,
                        DisplayTitle = y.DisplayTitle,
                        IsDefault = y.IsDefault,
                        MovieId = video.Id
                    }).ToList();

                video.VideoStreams = dto.MediaStreams
                    .Where(y => y.Type == MediaStreamType.Video)
                    .Select(y => new SqlVideoStream
                    {
                        Language = y.Language,
                        BitRate = y.BitRate,
                        AspectRatio = y.AspectRatio,
                        AverageFrameRate = y.AverageFrameRate,
                        Channels = y.Channels,
                        Height = y.Height,
                        Width = y.Width,
                        BitDepth = y.BitDepth,
                        Codec = y.Codec,
                        IsDefault = y.IsDefault,
                        VideoRange = y.VideoRange,
                        MovieId = video.Id
                    }).ToList();
            }
            else
            {
                video.AudioStreams = new List<AudioStream>(0);
                video.SubtitleStreams = new List<SubtitleStream>(0);
                video.VideoStreams = new List<SqlVideoStream>(0);
            }

            return video;
        }

        public static T MapMediaSources<T>(this BaseItemDto dto, T video) where T : Movie
        {
            if (dto.MediaSources != null)
            {
                video.MediaSources = dto.MediaSources
                    .Select(y => new MediaSource
                    {
                        Path = y.Path,
                        BitRate = y.Bitrate,
                        Container = y.Container,
                        Protocol = y.Protocol.ToString(),
                        RunTimeTicks = y.RunTimeTicks,
                        SizeInMb = Math.Round(y.Size / (double) 1024 / 1024 ?? 0, MidpointRounding.AwayFromZero),
                        MovieId = video.Id
                    }).ToList();
            }
            else
            {
                video.MediaSources = new List<MediaSource>(0);
            }

            return video;
        }

        public static Movie MapPeople(this BaseItemDto dto, Movie extra)
        {
            if (dto.People == null || !dto.People.Any())
            {
                return extra;
            }

            extra.People ??= new List<MediaPerson>();
            foreach (var person in dto.People)
            {
                extra.People.Add(new MediaPerson
                {
                    MovieId = extra.Id,
                    PersonId = person.Id,
                    Type = person.Type,
                });
            }

            return extra;
        }

        public static T MapProviderIds<T>(this BaseItemDto dto, T extra) where T : Movie
        {
            if (dto.ProviderIds == null)
            {
                return extra;
            }

            if (int.TryParse(dto.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value ?? string.Empty, out var tmdbValue))
            {
                extra.TMDB = tmdbValue;
            }

            extra.IMDB = dto.ProviderIds.FirstOrDefault(y => y.Key == "Imdb").Value ?? string.Empty;
            extra.TVDB = dto.ProviderIds.FirstOrDefault(y => y.Key == "Tvdb").Value ?? string.Empty;

            return extra;
        }

        public static T MapImageTags<T>(this BaseItemDto dto, T extra) where T : Movie
        {
            if (dto.ImageTags == null)
            {
                return extra;
            }

            extra.Primary = dto.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value ?? string.Empty;
            extra.Thumb = dto.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value ?? string.Empty;
            extra.Logo = dto.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value ?? string.Empty;
            extra.Banner = dto.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value ?? string.Empty;

            return extra;
        }
    }
}
