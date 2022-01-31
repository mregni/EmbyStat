using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Common.SqLite.Streams;

namespace EmbyStat.Common.Extensions
{
    public static class BaseItemDtoExtension
    {
        public static T MapStreams<T>(this BaseItemDto dto, T video) where T : SqlMovie
        {
            if (dto.MediaStreams != null)
            {
                video.AudioStreams = dto.MediaStreams
                    .Where(y => y.Type == MediaStreamType.Audio)
                    .Select(y => new SqlAudioStream
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
                    .Select(y => new SqlSubtitleStream
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
                video.AudioStreams = new List<SqlAudioStream>(0);
                video.SubtitleStreams = new List<SqlSubtitleStream>(0);
                video.VideoStreams = new List<SqlVideoStream>(0);
            }

            return video;
        }

        public static T MapMediaSources<T>(this BaseItemDto dto, T video) where T : SqlMovie
        {
            if (dto.MediaSources != null)
            {
                video.MediaSources = dto.MediaSources
                    .Select(y => new SqlMediaSource
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
                video.MediaSources = new List<SqlMediaSource>(0);
            }

            return video;
        }

        public static T MapPeople<T>(this BaseItemDto dto, T extra) where T : SqlMovie
        {
            if (dto.People == null || !dto.People.Any())
            {
                return extra;
            }

            extra.MoviePeople ??= new List<SqlMoviePerson>();
            foreach (var person in dto.People)
            {
                extra.MoviePeople.Add(new SqlMoviePerson
                {
                    MovieId = extra.Id,
                    PersonId = person.Id,
                    Type = person.Type,
                });
            }

            return extra;
        }

        public static T MapProviderIds<T>(this BaseItemDto dto, T extra) where T : SqlMovie
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

        public static T MapImageTags<T>(this BaseItemDto dto, T extra) where T : SqlMovie
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

        public static T MapGenres<T>(this BaseItemDto dto, T extra, List<SqlGenre> genres) where T : SqlMovie
        {
            if (dto.Genres == null || !dto.Genres.Any())
            {
                return extra;
            }

            extra.Genres ??= new List<SqlGenre>();
            foreach (var dtoGenre in dto.Genres)
            {
                var localGenre = genres.FirstOrDefault(x => x.Name == dtoGenre);
                extra.Genres.Add(localGenre);
            }

            return extra;
        }
    }
}
