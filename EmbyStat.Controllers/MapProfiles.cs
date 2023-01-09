﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.DataGrid;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Entities.Streams;
using EmbyStat.Common.Models.Entities.Users;
using EmbyStat.Common.Models.MediaServer;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.Models.Sessions;
using EmbyStat.Common.Models.Show;
using EmbyStat.Configuration;
using EmbyStat.Controllers.About;
using EmbyStat.Controllers.Filters;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.HelperClasses.Streams;
using EmbyStat.Controllers.Job;
using EmbyStat.Controllers.Log;
using EmbyStat.Controllers.Movie;
using EmbyStat.Controllers.Settings;
using EmbyStat.Controllers.Show;
using EmbyStat.Controllers.System;
using EmbyStat.Controllers.MediaServer;
using EmbyStat.Core.About;
using EmbyStat.Core.MediaServers;
using EmbyStat.Core.Movies;
using EmbyStat.Core.Shows;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.System;
using TMDbLib.Objects.Search;
using LogFile = EmbyStat.Core.Logging.LogFile;

namespace EmbyStat.Controllers;

public class MapProfiles : Profile
{
    public MapProfiles()
    {
        CreateShowMappings();
        CreateMovieMappings();
        CreateVideoMappings();
        CreatePeopleMappings();
        CreateMediaServerMappings();
        CreateSettingMappings();
        CreateLibraryMappings();
        CreateMediaServerUserMappings();
        CreateEventMappings();

        //USED MAPPINGS
        CreateMap<MediaServerSettings, MediaServerSettingsViewModel>().ReverseMap();
        CreateMap<TmdbSettings, TmdbSettingsViewModel>().ReverseMap();
        CreateMap<Language, LanguageViewModel>();

        CreateMap<Common.Models.Entities.Job, JobViewModel>()
            .ForMember(dest => dest.StartTimeUtcIso,
                src => src.MapFrom((org, _) =>
                    org.StartTimeUtc?.ToString("O") ?? string.Empty))
            .ForMember(dest => dest.EndTimeUtcIso,
                src => src.MapFrom((org, _) =>
                    org.EndTimeUtc?.ToString("O") ?? string.Empty));

        CreateMap<Card, CardViewModel>()
            .ForMember("Type", src => src.MapFrom((org, _) =>
            {
                return ((Card) org).Type switch
                {
                    CardType.Text => "text",
                    CardType.Size => "size",
                    CardType.Time => "time",
                    _ => "text"
                };
            }));

        CreateMap<Library, LibraryViewModel>()
            .ForMember(x => x.Sync, x => x.Ignore());
        CreateMap<Chart, ChartViewModel>();

        CreateMap<SystemInfo, MediaServerInfo>()
            .ReverseMap()
            .ForMember(x => x.CompletedInstallations, y => y.Ignore());

        CreateMap(typeof(Page<>), typeof(PageViewModel<>));
        CreateMap<AudioStream, AudioStreamViewModel>();
        CreateMap<MediaSource, MediaSourceViewModel>();
        CreateMap<SubtitleStream, SubtitleStreamViewModel>();
        CreateMap<VideoStream, VideoStreamViewModel>();
        CreateMap<BaseItemDto, Genre>();

        CreateMap<TopCard, TopCardViewModel>();
        CreateMap<TopCardItem, TopCardItemViewModel>();
        CreateMap<LabelValuePair, LabelValuePairViewModel>();
        CreateMap<FilterValues, FilterValuesViewModel>();
        CreateMap<MediaServerStatus, EmbyStatusViewModel>();

        CreateMap<LogFile, LogFileViewModel>();

        //TODO: NOT USED

        CreateMap<MediaServerUdpBroadcast, UdpBroadcastViewModel>().ReverseMap();
        CreateMap<UpdateResult, UpdateResultViewModel>();

        CreateMap<ShortMovie, ShortMovieViewModel>();
        CreateMap<SuspiciousTables, SuspiciousTablesViewModel>();

        CreateMap<AboutModel, AboutViewModel>();
        CreateMap<SuspiciousMovie, SuspiciousMovieViewModel>();

        CreateMap<UserMediaView, UserMediaViewViewModel>();

        CreateMap(typeof(ListContainer<>), typeof(ListContainer<>));
    }

    private void CreateMediaServerUserMappings()
    {
        CreateMap<BaseUserDto, MediaServerUser>()
            .ForMember(x => x.Views, x => x.Ignore())
            .ForMember(x => x.IsAdministrator, x => x.MapFrom(y => y.Policy.IsAdministrator))
            .ForMember(x => x.IsDisabled, x => x.MapFrom(y => y.Policy.IsDisabled))
            .ForMember(x => x.IsHidden, x => x.MapFrom(y => y.Policy.IsHidden))
            .ForMember(x => x.IsHiddenRemotely, x => x.MapFrom(y => y.Policy.IsHiddenRemotely))
            .ForMember(x => x.IsHiddenFromUnusedDevices, x => x.MapFrom(y => y.Policy.IsHiddenFromUnusedDevices))
            .ForMember(x => x.EnableLiveTvAccess, x => x.MapFrom(y => y.Policy.IsHiddenFromUnusedDevices))
            .ForMember(x => x.EnableLiveTvAccess, x => x.MapFrom(y => y.Policy.EnableLiveTvAccess))
            .ForMember(x => x.EnableContentDeletion, x => x.MapFrom(y => y.Policy.EnableContentDeletion))
            .ForMember(x => x.EnableContentDownloading, x => x.MapFrom(y => y.Policy.EnableContentDownloading))
            .ForMember(x => x.EnableSubtitleDownloading, x => x.MapFrom(y => y.Policy.EnableSubtitleDownloading))
            .ForMember(x => x.EnableSubtitleManagement, x => x.MapFrom(y => y.Policy.EnableSubtitleManagement))
            .ForMember(x => x.EnableSyncTranscoding, x => x.MapFrom(y => y.Policy.EnableSyncTranscoding))
            .ForMember(x => x.EnableMediaConversion, x => x.MapFrom(y => y.Policy.EnableMediaConversion))
            .ForMember(x => x.InvalidLoginAttemptCount, x => x.MapFrom(y => y.Policy.InvalidLoginAttemptCount))
            .ForMember(x => x.EnablePublicSharing, x => x.MapFrom(y => y.Policy.EnablePublicSharing))
            .ForMember(x => x.RemoteClientBitrateLimit, x => x.MapFrom(y => y.Policy.RemoteClientBitrateLimit))
            .ForMember(x => x.SimultaneousStreamLimit, x => x.MapFrom(y => y.Policy.SimultaneousStreamLimit))
            .ForMember(x => x.EnableAllDevices, x => x.MapFrom(y => y.Policy.EnableAllDevices))
            .ForMember(x => x.PlayDefaultAudioTrack, x => x.MapFrom(y => y.Configuration.PlayDefaultAudioTrack))
            .ForMember(x => x.SubtitleLanguagePreference,
                x => x.MapFrom(y => y.Configuration.SubtitleLanguagePreference))
            .ForMember(x => x.DisplayMissingEpisodes, x => x.MapFrom(y => y.Configuration.DisplayMissingEpisodes))
            .ForMember(x => x.SubtitleMode, x => x.MapFrom(y => y.Configuration.SubtitleMode));

        CreateMap<UserMediaView, UserMediaViewViewModel>();
        CreateMap<MediaServerUserRow, MediaServerUserRowViewModel>();
        CreateMap<MediaServerUser, UserOverviewViewModel>();
        CreateMap<MediaServerUser, UserFullViewModel>();

        CreateMap<MediaServerUserStatistics, MediaServerUserStatisticsViewModel>();

        CreateMap<BaseItemDto, MediaServerUserView>()
            .ForMember(x => x.MediaId, x => x.MapFrom(y => y.Id))
            .ForMember(x => x.MediaType, x => x.MapFrom(y => MapMediaType(y.Type)))
            .ForMember(x => x.PlayCount, x => x.MapFrom(y => y.UserData.PlayCount))
            .ForMember(x => x.UserId, x => x.Ignore())
            .ForMember(x => x.LastPlayedDate, x => x.MapFrom(y => y.UserData.LastPlayedDate));
    }

    private void CreateSettingMappings()
    {
        CreateMap<Config, ConfigViewModel>();
        CreateMap<UserConfig, UserConfigViewModel>().ReverseMap();
        CreateMap<SystemConfig, SystemConfigViewModel>();
        CreateMap<Hosting, HostingViewModel>().ReverseMap();
        CreateMap<MediaServerSettings, MediaServerSettingsViewModel>().ReverseMap();
        CreateMap<TmdbSettings, TmdbSettingsViewModel>().ReverseMap();
        CreateMap<Configuration.Rollbar, RollbarViewModel>();
        CreateMap<Dirs, DirsViewModel>();
    }

    private void CreatePeopleMappings()
    {
        CreateMap<BaseItemDto, Person>()
            .ForMember(x => x.Primary, x => x.MapFrom(y => y.ImageTags.ConvertToImageTag()));

        CreateMap<BaseItemPerson, MediaPerson>()
            .ForMember(x => x.PersonId, x => x.MapFrom(y => y.Id))
            .ForMember(x => x.Id, x => x.Ignore())
            .ForMember(x => x.MovieId, x => x.Ignore())
            .ForMember(x => x.Movie, x => x.Ignore())
            .ForMember(x => x.ShowId, x => x.Ignore())
            .ForMember(x => x.Show, x => x.Ignore());
    }

    private void CreateShowMappings()
    {
        CreateMap<BaseItemDto, Common.Models.Entities.Shows.Show>()
            .ForMember(x => x.Seasons, x => x.Ignore())
            .ForMember(x => x.CumulativeRunTimeTicks, x => x.Ignore())
            .ForMember(x => x.ExternalSynced, x => x.Ignore())
            .ForMember(x => x.SizeInMb, x => x.Ignore())
            .ForMember(x => x.Seasons, x => x.MapFrom((y => new List<Season>())))
            .AddImageMappings()
            .AddProviderMappings()
            .AddCommunityRatingMappings()
            .AddGenreMappings();

        CreateMap<BaseItemDto, Season>()
            .ForMember(x => x.Episodes, x => x.MapFrom((y => new List<Episode>())))
            .ForMember(x => x.ShowId, x => x.MapFrom(y => y.ParentId))
            .AddImageMappings();
        CreateMap<BaseItemDto, Episode>()
            .ForMember(x => x.SeasonId, x => x.MapFrom(y => y.ParentId))
            .ForMember(x => x.ShowId, x => x.MapFrom(y => y.SeriesId))
            .AddImageMappings()
            .AddProviderMappings()
            .AddCommunityRatingMappings()
            .AddStreamMappings();

        CreateMap<Common.Models.Entities.Shows.Show, ShowRowViewModel>()
            .ForMember(x => x.SeasonCount, x => x.MapFrom(y => y.Seasons.Count))
            .ForMember(x => x.EpisodeCount, x => x.MapFrom(y => y.Seasons
                .Where(z => z.IndexNumber != 0)
                .SelectMany(z => z.Episodes)
                .Count(z => z.LocationType == LocationType.Disk)))
            .ForMember(x => x.SpecialEpisodeCount, x => x.MapFrom(y => y.Seasons
                .Where(z => z.IndexNumber == 0)
                .SelectMany(z => z.Episodes)
                .Count(z => z.LocationType == LocationType.Disk)))
            .ForMember(x => x.MissingEpisodeCount, x => x.MapFrom(y => y.Seasons
                .Where(z => z.IndexNumber != 0)
                .SelectMany(z => z.Episodes)
                .Count(z => z.LocationType == LocationType.Virtual)))
            .ForMember(x => x.Genres, x => x.MapFrom(y => y.Genres.Select(z => z.Name).Distinct()))
            .ForMember(x => x.RunTime, x => x.MapFrom(y => Math.Round((decimal) (y.RunTimeTicks ?? 0) / 600000000)))
            .ForMember(x => x.CumulativeRunTime,
                x => x.MapFrom(y => Math.Round((decimal) (y.CumulativeRunTimeTicks ?? 0) / 600000000)));

        CreateMap<Common.Models.Entities.Shows.Show, ShowDetailViewModel>()
            .ForMember(x => x.SeasonCount, x => x.MapFrom(y => y.Seasons.Count))
            .ForMember(x => x.EpisodeCount, x => x.MapFrom(y => y.Seasons
                .Where(z => z.IndexNumber != 0)
                .SelectMany(z => z.Episodes)
                .Count(z => z.LocationType == LocationType.Disk)))
            .ForMember(x => x.SpecialEpisodeCount, x => x.MapFrom(y => y.Seasons
                .Where(z => z.IndexNumber == 0)
                .SelectMany(z => z.Episodes)
                .Count(z => z.LocationType == LocationType.Disk)))
            .ForMember(x => x.Genres, x => x.MapFrom(y => y.Genres.Select(z => z.Name).Distinct()))
            .ForMember(x => x.RunTime, x => x.MapFrom(y => Math.Round((decimal) (y.RunTimeTicks ?? 0) / 600000000)))
            .ForMember(x => x.CumulativeRunTime,
                x => x.MapFrom(y => Math.Round((decimal) (y.CumulativeRunTimeTicks ?? 0) / 600000000)))
            .ForMember(x => x.MissingSeasons,
                x => x.MapFrom(y => y.Seasons.Where(s =>
                    s.IndexNumber != 0 && s.Episodes.Any(e => e.LocationType == LocationType.Virtual))));

        CreateMap<Season, VirtualSeasonViewModel>()
            .ForMember(x => x.Episodes,
                x => x.MapFrom(y => y.Episodes.Where(z => z.LocationType == LocationType.Virtual)));
        CreateMap<Episode, VirtualEpisodeViewModel>();
        CreateMap<TvSeasonEpisode, VirtualEpisode>()
            .ForMember(x => x.Id, x => x.MapFrom(y => y.Id.ToString()))
            .ForMember(x => x.FirstAired, x => x.MapFrom(y => y.AirDate));

        CreateMap<TvMaze.Api.Client.Models.Episode, VirtualEpisode>()
            .ForMember(x => x.Id, x => x.MapFrom(y => y.Id.ToString()))
            .ForMember(x => x.FirstAired,
                x => x.MapFrom(y => y.AirStamp != null ? y.AirStamp.Value.DateTime : (DateTime?) null))
            .ForMember(x => x.SeasonNumber, x => x.MapFrom(y => y.Season))
            .ForMember(x => x.EpisodeNumber, x => x.MapFrom(y => y.Number ?? 0));


        CreateMap<ShowCharts, ShowChartsViewModel>();
        CreateMap<ShowStatistics, ShowStatisticsViewModel>();
    }

    private void CreateMovieMappings()
    {
        CreateMap<QueryResult<BaseItemDto>, QueryResult<Common.Models.Entities.Movies.Movie>>();
        CreateMap<Common.Models.Entities.Movies.Movie, MovieRowViewModel>()
            .ForMember(x => x.AudioLanguages,
                x => x.MapFrom(y => y.AudioStreams.Select(z => z.Language).Distinct()))
            .ForMember(x => x.Genres, x => x.MapFrom(y => y.Genres.Select(z => z.Name).Distinct()))
            .ForMember(x => x.RunTime, x => x.MapFrom(y => Math.Round((decimal) (y.RunTimeTicks ?? 0) / 600000000)))
            .ForMember(x => x.Subtitles, x => x.MapFrom(y => y.SubtitleStreams.Select(z => z.Language).Distinct()))
            .ForMember(x => x.SizeInMb,
                x => x.MapFrom(y => y.MediaSources.FirstOrDefault() != null ? y.MediaSources.First().SizeInMb : 0));
        CreateMap<BaseItemDto, Common.Models.Entities.Movies.Movie>()
            .ForMember(x => x.Video3DFormat, x => x.MapFrom(y => y.Video3DFormat ?? 0))
            .AddImageMappings()
            .AddProviderMappings()
            .AddGenreMappings()
            .AddCommunityRatingMappings()
            .AddStreamMappings();
        CreateMap<Common.Models.Entities.Movies.Movie, MovieViewModel>()
            .ForMember(x => x.Genres, x => x.MapFrom(y => y.Genres.Select(z => z.Name).Distinct()))
            .ForMember(x => x.RunTime, x => x.MapFrom(y => Math.Round((decimal) (y.RunTimeTicks ?? 0) / 600000000)))
            ;

        CreateMap<MovieStatistics, MovieStatisticsViewModel>();
    }


    private void CreateMediaServerMappings()
    {
        CreateMap<PluginInfo, PluginViewModel>();
        CreateMap<MediaServerInfo, ServerInfoViewModel>()
            .ForMember(x => x.ActiveUserCount, x => x.Ignore())
            .ForMember(x => x.IdleUserCount, x => x.Ignore())
            .ForMember(x => x.ActiveDeviceCount, x => x.Ignore())
            .ForMember(x => x.IdleDeviceCount, x => x.Ignore());
    }

    private void CreateVideoMappings()
    {
        CreateMap<BaseMediaStream, AudioStream>()
            .ForMember(x => x.Id, x => x.MapFrom(y => Guid.NewGuid().ToString()));
        CreateMap<BaseMediaStream, SubtitleStream>()
            .ForMember(x => x.Id, x => x.MapFrom(y => Guid.NewGuid().ToString()));
        CreateMap<BaseMediaStream, VideoStream>()
            .ForMember(x => x.Id, x => x.MapFrom(y => Guid.NewGuid().ToString()))
            .ForMember(x => x.AverageFrameRate,
                x => x.MapFrom(y =>
                    y.AverageFrameRate.HasValue ? (float) Math.Round(y.AverageFrameRate.Value, 2) : (float?) null));
        CreateMap<BaseMediaSourceInfo, MediaSource>()
            .ForMember(x => x.Protocol, x => x.MapFrom(y => y.ToString()))
            .ForMember(x => x.SizeInMb,
                x => x.MapFrom(y => Math.Round(y.Size / (double) 1024 / 1024 ?? 0, MidpointRounding.AwayFromZero)));
    }

    private void CreateLibraryMappings()
    {
        CreateMap<BaseItemDto, Library>()
            .ForMember(x => x.Type, x => x.MapFrom(y => y.CollectionType.ToLibraryType()))
            .ForMember(x => x.Primary, x => x.MapFrom(y => y.ImageTags.ConvertToImageTag()));
    }

    private void CreateEventMappings()
    {
        CreateMap<WebSocketSession, Session>();
        CreateMap<WebSocketSession, MediaPlay>()
            .ForMember(x => x.Id, x => x.Ignore())
            .ForMember(x => x.SessionId, x => x.MapFrom(y => y.Id))
            .ForMember(x => x.UserId, x => x.MapFrom(y => y.UserId))
            .ForMember(x => x.MediaId, x => x.MapFrom(y => y.NowPlayingItem.Id))
            .ForMember(x => x.Type, x => x.MapFrom(y => y.NowPlayingItem.Type))
            .ForMember(x => x.PlayMethod, x => x.MapFrom(y => y.PlayState.PlayMethod))
            .ForMember(x => x.Start, x => x.Ignore())
            .ForMember(x => x.LastUpdate, x => x.MapFrom(y => y.LastActivityDate))
            .ForMember(x => x.Stop, x => x.Ignore())
            .ForMember(x => x.StartPositionTicks, x => x.Ignore())
            .ForMember(x => x.EndPositionTicks, x => x.MapFrom(y => y.PlayState.PositionTicks))
            .ForMember(x => x.IsPaused, x => x.MapFrom(y => y.PlayState.IsPaused))
            .ForMember(x => x.AudioCodec, x => x.MapFrom(y => y.NowPlayingItem.MediaStreams.GetStreamValue(y.PlayState.AudioStreamIndex, x => x.Codec)))
            .ForMember(x => x.AudioChannelLayout, x => x.MapFrom(y => y.NowPlayingItem.MediaStreams.GetStreamValue(y.PlayState.AudioStreamIndex, x => x.ChannelLayout)))
            .ForMember(x => x.AudioSampleRate, x => x.MapFrom(y => y.NowPlayingItem.MediaStreams.GetStreamValue(y.PlayState.AudioStreamIndex, x => x.SampleRate)))
            .ForMember(x => x.SubtitleCodec, x => x.MapFrom(y => y.NowPlayingItem.MediaStreams.GetStreamValue(y.PlayState.SubtitleStreamIndex, x => x.Codec)))
            .ForMember(x => x.SubtitleDisplayLanguage, x => x.MapFrom(y => y.NowPlayingItem.MediaStreams.GetStreamValue(y.PlayState.SubtitleStreamIndex, x => x.DisplayLanguage)))
            .ForMember(x => x.SubtitleLanguage, x => x.MapFrom(y => y.NowPlayingItem.MediaStreams.GetStreamValue(y.PlayState.SubtitleStreamIndex, x => x.Language)))
            .ForMember(x => x.SubtitleProtocol, x => x.MapFrom(y => y.NowPlayingItem.MediaStreams.GetStreamValue(y.PlayState.SubtitleStreamIndex, x => x.Protocol)))
            .ForMember(x => x.TranscodeAverageCpuUsage, x => x.MapFrom(y => y.TranscodingInfo != null ? y.TranscodingInfo.AverageCpuUsage : (double?)null))
            .ForMember(x => x.TranscodeCurrentCpuUsage, x => x.MapFrom(y => y.TranscodingInfo != null ? y.TranscodingInfo.CurrentCpuUsage : (double?)null))
            .ForMember(x => x.TranscodeVideoCodec, x => x.MapFrom(y => y.TranscodingInfo != null ? y.TranscodingInfo.VideoCodec : null))
            .ForMember(x => x.TranscodeAudioCodec, x => x.MapFrom(y => y.TranscodingInfo != null ? y.TranscodingInfo.AudioCodec : null))
            .ForMember(x => x.TranscodeSubProtocol, x => x.MapFrom(y => y.TranscodingInfo != null ? y.TranscodingInfo.SubProtocol : null))
            .ForMember(x => x.TranscodeReasons, x => x.MapFrom(y => y.TranscodingInfo != null ? string.Join(";", y.TranscodingInfo.TranscodeReasons) : null))
            .ForMember(x => x.Encoder, x => x.MapFrom(y => y.TranscodingInfo != null ? y.TranscodingInfo.VideoEncoder : null))
            .ForMember(x => x.EncoderIsHardware, x => x.MapFrom(y => y.TranscodingInfo != null ? y.TranscodingInfo.VideoEncoderIsHardware : (bool?)null))
            .ForMember(x => x.EncoderMediaType, x => x.MapFrom(y => y.TranscodingInfo != null ? y.TranscodingInfo.VideoEncoderMediaType : null))
            .ForMember(x => x.Decoder, x => x.MapFrom(y => y.TranscodingInfo != null ? y.TranscodingInfo.VideoDecoder : null))
            .ForMember(x => x.DecoderIsHardware, x => x.MapFrom(y => y.TranscodingInfo != null ? y.TranscodingInfo.VideoDecoderIsHardware : (bool?)null))
            .ForMember(x => x.DecoderMediaType, x => x.MapFrom(y => y.TranscodingInfo != null ? y.TranscodingInfo.VideoDecoderMediaType : null))
            .ForMember(x => x.WatchedPercentage, x => x.MapFrom(y => Math.Round(y.PlayState.PositionTicks/(double)y.NowPlayingItem.RunTimeTicks, 4)*100));
    }
    
    private static MediaType MapMediaType(string type)
    {
        return type switch
        {
            "Movie" => MediaType.Movie,
            "Episode" => MediaType.Episode,
            _ => MediaType.Unknown
        };
    }
}

public static class MediaStreamMappingExtensions {
    public static T GetStreamValue<T>(this IReadOnlyList<MediaStream> list, int index, Func<MediaStream, T> action)
    {
        if (index < 0)
        {
            return default;
        }

        if (index > list.Count)
        {
            return default;
        }

        return action(list[index]);
    }
}

public static class DictionaryMappingExtensions
{
    public static string ConvertToImageTag(this Dictionary<ImageType, string> tags)
    {
        return tags.ContainsKey(ImageType.Primary)
            ? tags.FirstOrDefault(y => y.Key == ImageType.Primary).Value
            : default;
    }
}

public static class SqlExtraMapperExtensions
{
    public static IMappingExpression<T1, T2> AddCommunityRatingMappings<T1, T2>(this IMappingExpression<T1, T2> mapping)
        where T1 : BaseItemDto where T2 : Extra
    {
        return mapping.ForMember(x => x.CommunityRating,
            x => x.MapFrom(y =>
                y.CommunityRating != null ? (float) Math.Round(y.CommunityRating.Value, 1) : (float?) null));
    }

    public static IMappingExpression<T1, T2> AddGenreMappings<T1, T2>(this IMappingExpression<T1, T2> mapping)
        where T1 : BaseItemDto where T2 : ILinkedMedia
    {
        return mapping.ForMember(x => x.Genres, x => x.MapFrom(y => y.Genres.Select(z => new Genre {Name = z})));
    }

    public static IMappingExpression<T1, T2> AddImageMappings<T1, T2>(this IMappingExpression<T1, T2> mapping)
        where T1 : BaseItemDto where T2 : Media
    {
        return mapping
            .ForMember(x => x.Primary,
                x => x.MapFrom(y =>
                    y.ImageTags.FirstOrDefault(z => z.Key == ImageType.Primary).Value ?? string.Empty))
            .ForMember(x => x.Thumb,
                x => x.MapFrom(y =>
                    y.ImageTags.FirstOrDefault(z => z.Key == ImageType.Thumb).Value ?? string.Empty))
            .ForMember(x => x.Logo,
                x => x.MapFrom(y => y.ImageTags.FirstOrDefault(z => z.Key == ImageType.Logo).Value ?? string.Empty))
            .ForMember(x => x.Banner,
                x => x.MapFrom(
                    y => y.ImageTags.FirstOrDefault(z => z.Key == ImageType.Banner).Value ?? string.Empty));
    }

    public static IMappingExpression<T1, T2> AddProviderMappings<T1, T2>(this IMappingExpression<T1, T2> mapping)
        where T1 : BaseItemDto where T2 : Extra
    {
        return mapping
            .ForMember(x => x.IMDB,
                x => x.MapFrom(y => y.ProviderIds.FirstOrDefault(z => z.Key == "Imdb").Value ?? string.Empty))
            .ForMember(x => x.TVDB,
                x => x.MapFrom(y => MapInt(y.ProviderIds.FirstOrDefault(z => z.Key == "Tvdb").Value ?? string.Empty)))
            .ForMember(x => x.TMDB,
                x => x.MapFrom(
                    y => MapInt(y.ProviderIds.FirstOrDefault(z => z.Key == "Tmdb").Value ?? string.Empty)));
    }

    public static void AddStreamMappings<T1, T2>(this IMappingExpression<T1, T2> mapping)
        where T1 : BaseItemDto where T2 : Video
    {
        mapping
            .ForMember(x => x.AudioStreams,
                x => x.MapFrom(y => y.MediaStreams.Where(z => z.Type == MediaStreamType.Audio)))
            .ForMember(x => x.VideoStreams,
                x => x.MapFrom(y => y.MediaStreams.Where(z => z.Type == MediaStreamType.Video)))
            .ForMember(x => x.SubtitleStreams,
                x => x.MapFrom(y => y.MediaStreams.Where(z => z.Type == MediaStreamType.Subtitle)));
    }

    private static int? MapInt(string input)
    {
        if (int.TryParse(input, out var tmdbValue))
        {
            return tmdbValue;
        }

        return null;
    }
}