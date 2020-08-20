using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Net;

namespace EmbyStat.Common.Extensions
{
    public static class BaseItemDtoExtension
    {
        public static T MapStreams<T>(this BaseItemDto dto, T video) where T : Video
        {
            if (dto.MediaStreams != null)
            {
                video.AudioStreams = dto.MediaStreams
                    .Where(y => y.Type == MediaStreamType.Audio)
                    .Select(y => new AudioStream
                    {
                        Id = Guid.NewGuid().ToString(),
                        BitRate = y.BitRate,
                        ChannelLayout = y.ChannelLayout,
                        Channels = y.Channels,
                        Codec = y.Codec,
                        Language = y.Language,
                        SampleRate = y.SampleRate,
                        IsDefault = y.IsDefault,
                    }).ToList();

                video.SubtitleStreams = dto.MediaStreams
                    .Where(y => y.Type == MediaStreamType.Subtitle)
                    .Select(y => new SubtitleStream
                    {
                        Id = Guid.NewGuid().ToString(),
                        Language = y.Language,
                        Codec = y.Codec,
                        DisplayTitle = y.DisplayTitle,
                        IsDefault = y.IsDefault
                    }).ToList();

                video.VideoStreams = dto.MediaStreams
                    .Where(y => y.Type == MediaStreamType.Video)
                    .Select(y => new VideoStream
                    {
                        Id = Guid.NewGuid().ToString(),
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
                        VideoRange = y.VideoRange
                    }).ToList();
            }
            else
            {
                video.AudioStreams = new List<AudioStream>(0);
                video.SubtitleStreams = new List<SubtitleStream>(0);
                video.VideoStreams = new List<VideoStream>(0);
            }

            return video;
        }

        public static T MapMediaSources<T>(this BaseItemDto dto, T video) where T : Video
        {
            if (dto.MediaSources != null)
            {
                video.MediaSources = dto.MediaSources
                    .Select(y => new MediaSource
                    {
                        Id = Guid.NewGuid().ToString(),
                        Path = y.Path,
                        BitRate = y.Bitrate,
                        Container = y.Container,
                        Protocol = y.Protocol.ToString(),
                        RunTimeTicks = y.RunTimeTicks,
                        SizeInMb = Math.Round(y.Size / (double) 1024 / 1024 ?? 0, MidpointRounding.AwayFromZero)
                    }).ToList();
            }
            else
            {
                video.MediaSources = new List<MediaSource>(0);
            }

            return video;
        }

        public static T MapPeople<T>(this BaseItemDto dto, T extra) where T : Extra
        {
            if (dto?.People != null)
            {
                extra.People = dto.People
                    .Where(y => y.Name.Length > 1)
                    .Select(y => new ExtraPerson
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Type = y.Type
                    }).ToArray();
            }
            else
            {
                extra.People = new ExtraPerson[0];
            }

            return extra;
        }

        public static T MapProviderIds<T>(this BaseItemDto dto, T extra) where T : Extra
        {
            if (dto.ProviderIds != null)
            {
                extra.IMDB = dto.ProviderIds.FirstOrDefault(y => y.Key == "Imdb").Value ?? string.Empty;
                extra.TMDB = dto.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value ?? string.Empty;
                extra.TVDB = dto.ProviderIds.FirstOrDefault(y => y.Key == "Tvdb").Value ?? string.Empty;
            }

            return extra;
        }

        public static T MapImageTags<T>(this BaseItemDto dto, T extra) where T : Media
        {
            if (dto.ImageTags != null)
            {
                extra.Primary = dto.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value ?? string.Empty;
                extra.Thumb = dto.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value ?? string.Empty;
                extra.Logo = dto.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value ?? string.Empty;
                extra.Banner = dto.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value ?? string.Empty;
            }

            return extra;
        }
    }
}
