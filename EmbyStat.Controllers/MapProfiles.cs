using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Common.Models.Show;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Helpers;
using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Common.SqLite.Streams;
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
using EmbyStat.Services.Models.Cards;
using EmbyStat.Services.Models.Charts;
using EmbyStat.Services.Models.DataGrid;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.System;
using LogFile = EmbyStat.Services.Models.Logs.LogFile;

namespace EmbyStat.Controllers
{
    public class MapProfiles : Profile
    {
        public MapProfiles()
        {
            CreateShowMappings();
            CreateMovieMappings();
            CreateVideoMappings();
            CreatePeopleMappings();

            CreateMap(typeof(Page<>), typeof(PageViewModel<>));




            CreateMap<MediaBrowser.Model.Plugins.PluginInfo, PluginInfo>();
            CreateMap<UserSettings, FullSettingsViewModel>()
                .ForMember(x => x.Version, x => x.Ignore())
                .ReverseMap()
                .ForMember(x => x.Version, x => x.Ignore())
                .AfterMap((src, dest) => dest.MediaServer.ApiKey = src.MediaServer.ApiKey.Trim());
            CreateMap<LibraryContainer, LibraryContainerViewModel>().ReverseMap();
            CreateMap<MediaServerSettings, MediaServerSettingsViewModel>().ReverseMap();
            CreateMap<TmdbSettings, TmdbSettingsViewModel>().ReverseMap();
            CreateMap<Language, LanguageViewModel>();
            CreateMap<MediaServerUdpBroadcast, UdpBroadcastViewModel>().ReverseMap();
            CreateMap<PluginInfo, PluginViewModel>();
            CreateMap<ServerInfo, ServerInfoViewModel>();
            CreateMap<UpdateResult, UpdateResultViewModel>();

            CreateMap<Common.Models.Entities.Job, JobViewModel>()
                .ForMember(dest => dest.StartTimeUtcIso,
                    src => src.MapFrom((org, x) =>
                        org.StartTimeUtc?.ToString("O") ?? string.Empty))
                .ForMember(dest => dest.EndTimeUtcIso,
                    src => src.MapFrom((org, x) =>
                        org.EndTimeUtc?.ToString("O") ?? string.Empty));

            CreateMap<TimeSpanCard, TimeSpanCardViewModel>();
            CreateMap(typeof(Card<>), typeof(CardViewModel<>))
                .ForMember("Type", src => src.MapFrom((org, x) =>
                {
                    return ((Card<string>)org).Type switch
                    {
                        CardType.Text => "text",
                        CardType.Size => "size",
                        CardType.Time => "time",
                        _ => "text"
                    };
                }));

            CreateMap<MoviePoster, MoviePosterViewModel>();
            CreateMap<ShowPoster, ShowPosterViewModel>();
            CreateMap<PersonPoster, PersonPosterViewModel>();
            CreateMap<MovieStatistics, MovieStatisticsViewModel>();
            CreateMap<ShowStatistics, ShowStatisticsViewModel>();
            CreateMap<PersonStats, PersonStatsViewModel>();
            CreateMap<Library, LibraryViewModel>();
            CreateMap<Chart, ChartViewModel>();
            CreateMap<ShowCharts, ShowChartsViewModel>();
            CreateMap<ShortMovie, ShortMovieViewModel>();
            CreateMap<SuspiciousTables, SuspiciousTablesViewModel>();
            CreateMap<ShowGeneral, ShowGeneralViewModel>();
            CreateMap<ShowCollectionRow, ShowCollectionRowViewModel>();
            CreateMap<LogFile, LogFileViewModel>();
            CreateMap<Services.Models.About.About, AboutViewModel>();
            CreateMap<SuspiciousMovie, SuspiciousMovieViewModel>();
            CreateMap<EmbyUser, UserIdViewModel>();
            CreateMap<EmbyUser, UserOverviewViewModel>();
            CreateMap<EmbyUser, UserFullViewModel>();
            CreateMap<UserMediaView, UserMediaViewViewModel>();
            CreateMap<VirtualSeason, VirtualSeasonViewModel>();
            CreateMap<VirtualEpisode, VirtualEpisodeViewModel>();
            CreateMap<SystemInfo, ServerInfo>()
                .ForMember(x => x.Id, y => Guid.NewGuid())
                .ReverseMap()
                .ForMember(x => x.CompletedInstallations, y => y.Ignore());
            CreateMap(typeof(ListContainer<>), typeof(ListContainer<>));

            CreateMap<TopCard, TopCardViewModel>();
            CreateMap<TopCardItem, TopCardItemViewModel>();
            CreateMap<LabelValuePair, LabelValuePairViewModel>();
            CreateMap<FilterValues, FilterValuesViewModel>();
            CreateMap<EmbyStatus, EmbyStatusViewModel>();
            

            CreateMap<SqlAudioStream, AudioStreamViewModel>();
            CreateMap<SqlMediaSource, MediaSourceViewModel>();
            CreateMap<SqlSubtitleStream, SubtitleStreamViewModel>();
            CreateMap<SqlVideoStream, VideoStreamViewModel>();
        }

        private void CreatePeopleMappings()
        {
            CreateMap<BaseItemPerson, SqlMediaPerson>()
                .ForMember(x => x.PersonId, x => x.MapFrom(y => y.Id))
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.MovieId, x => x.Ignore())
                .ForMember(x => x.Movie, x => x.Ignore())
                .ForMember(x => x.ShowId, x => x.Ignore())
                .ForMember(x => x.Show, x => x.Ignore());
        }

        private void CreateShowMappings()
        {
            CreateMap<BaseItemDto, SqlShow>()
                .ForMember(x => x.Seasons, x => x.Ignore())
                .ForMember(x => x.CumulativeRunTimeTicks, x => x.Ignore())
                .ForMember(x => x.ExternalSynced, x => x.Ignore())
                .ForMember(x => x.SizeInMb, x => x.Ignore())
                .ForMember(x => x.Seasons, x => x.MapFrom((y => new List<SqlSeason>())))
                .AddImageMappings()
                .AddProviderMappings()
                .AddCommunityRatingMappings()
                .AddGenreMappings();

            CreateMap<SqlShow, ShowRowViewModel>()
                .ForMember(x => x.SeasonCount, x => x.MapFrom(y => y.Seasons.Count))
                .ForMember(x => x.EpisodeCount, x => x.MapFrom(y => y.Seasons
                    .Where(z => z.IndexNumber != 0)
                    .SelectMany(z => z.Episodes)
                    .Where(z => z.LocationType == LocationType.Disk)))
                .ForMember(x => x.SpecialEpisodeCount, x => x.MapFrom(y => y.Seasons
                    .Where(z => z.IndexNumber == 0)
                    .SelectMany(z => z.Episodes)
                    .Where(z => z.LocationType == LocationType.Disk)))
                .ForMember(x => x.MissingEpisodeCount, x => x.MapFrom(y => y.Seasons
                    .Where(z => z.IndexNumber != 0)
                    .SelectMany(z => z.Episodes)
                    .Where(z => z.LocationType == LocationType.Virtual)))
                .ForMember(x => x.Genres, x => x.MapFrom(y => y.Genres.Select(x => x.Name).Distinct()))
                .ForMember(x => x.RunTime, x => x.MapFrom(y => Math.Round((decimal)(y.RunTimeTicks ?? 0) / 600000000)));

            CreateMap<BaseItemDto, SqlSeason>()
                .ForMember(x => x.Episodes, x => x.MapFrom((y => new List<SqlEpisode>())))
                .ForMember(x => x.ShowId, x => x.MapFrom(y => y.ParentId))
                .AddImageMappings();
            CreateMap<BaseItemDto, SqlEpisode>()
                .ForMember(x => x.SeasonId, x => x.MapFrom(y => y.ParentId))
                .AddImageMappings()
                .AddProviderMappings()
                .AddCommunityRatingMappings()
                .AddStreamMappings();
        }

        private void CreateMovieMappings()
        {
            CreateMap<QueryResult<BaseItemDto>, QueryResult<SqlMovie>>();
            CreateMap<SqlMovie, MovieRowViewModel>()
                .ForMember(x => x.AudioLanguages,
                    x => x.MapFrom(y => y.AudioStreams.Select(x => x.Language).Distinct()))
                .ForMember(x => x.Genres, x => x.MapFrom(y => y.Genres.Select(x => x.Name).Distinct()))
                .ForMember(x => x.RunTime, x => x.MapFrom(y => Math.Round((decimal) (y.RunTimeTicks ?? 0) / 600000000)))
                .ForMember(x => x.Subtitles, x => x.MapFrom(y => y.SubtitleStreams.Select(z => z.Language).Distinct()))
                .ForMember(x => x.SizeInMb,
                    x => x.MapFrom(y => y.MediaSources.FirstOrDefault() != null ? y.MediaSources.First().SizeInMb : 0));
            CreateMap<BaseItemDto, SqlMovie>()
                .ForMember(x => x.CollectionId, x => x.MapFrom(y => y.ParentId))
                .ForMember(x => x.Video3DFormat, x => x.MapFrom(y => y.Video3DFormat ?? 0))
                .AddImageMappings()
                .AddProviderMappings()
                .AddGenreMappings()
                .AddCommunityRatingMappings()
                .AddStreamMappings();
            CreateMap<SqlMovie, MovieViewModel>()
                .ForMember(x => x.Genres, x => x.MapFrom(y => y.Genres.Select(z => z.Name).Distinct()));
        }

        private void CreateVideoMappings()
        {
            CreateMap<BaseMediaStream, SqlAudioStream>()
                .ForMember(x => x.Id, x => x.MapFrom(y => Guid.NewGuid().ToString()));
            CreateMap<BaseMediaStream, SqlSubtitleStream>()
                .ForMember(x => x.Id, x => x.MapFrom(y => Guid.NewGuid().ToString()));
            CreateMap<BaseMediaStream, SqlVideoStream>()
                .ForMember(x => x.Id, x => x.MapFrom(y => Guid.NewGuid().ToString()))
                .ForMember(x => x.AverageFrameRate, x => x.MapFrom(y => y.AverageFrameRate.HasValue ? (float)Math.Round(y.AverageFrameRate.Value, 2): (float?)null));
            CreateMap<BaseMediaSourceInfo, SqlMediaSource>()
                .ForMember(x => x.Protocol, x => x.MapFrom(y => y.ToString()))
                .ForMember(x => x.SizeInMb, x => x.MapFrom(y => Math.Round(y.Size / (double)1024 / 1024 ?? 0, MidpointRounding.AwayFromZero)));
        }
    }

    public static class SqlExtraMapperExtensions
    {
        public static IMappingExpression<T1, T2> AddCommunityRatingMappings<T1, T2>(this IMappingExpression<T1, T2> mapping)
        where T1 : BaseItemDto where T2 : SqlExtra
        {
            return mapping.ForMember(x => x.CommunityRating,
                x => x.MapFrom(y =>
                    y.CommunityRating != null ? (float) Math.Round(y.CommunityRating.Value, 1) : (float?) null));

        }
    public static IMappingExpression<T1, T2> AddGenreMappings<T1, T2>(this IMappingExpression<T1, T2> mapping)
            where T1 : BaseItemDto where T2 : ISqlLinked
        {
            return mapping.ForMember(x => x.Genres, x => x.MapFrom(y => y.Genres.Select(z => new SqlGenre { Name = z })));
        }

        public static IMappingExpression<T1, T2> AddImageMappings<T1, T2>(this IMappingExpression<T1, T2> mapping)
            where T1 : BaseItemDto where T2 : SqlMedia
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
            where T1 : BaseItemDto where T2 : SqlExtra
        {
            return mapping
                .ForMember(x => x.IMDB,
                    x => x.MapFrom(y => y.ProviderIds.FirstOrDefault(z => z.Key == "Imdb").Value ?? string.Empty))
                .ForMember(x => x.TVDB,
                    x => x.MapFrom(y => y.ProviderIds.FirstOrDefault(z => z.Key == "Tvdb").Value ?? string.Empty))
                .ForMember(x => x.TMDB,
                    x => x.MapFrom(
                        y => MapInt(y.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value ?? string.Empty)));
        }

        public static IMappingExpression<T1, T2> AddStreamMappings<T1, T2>(this IMappingExpression<T1, T2> mapping)
            where T1 : BaseItemDto where T2 : SqlVideo
        {
            return mapping
                .ForMember(x => x.AudioStreams,
                    x => x.MapFrom(y => y.MediaStreams.Where(z => z.Type == MediaStreamType.Audio)))
                .ForMember(x => x.VideoStreams,
                    x => x.MapFrom(y => y.MediaStreams.Where(z => z.Type == MediaStreamType.Video)))
                .ForMember(x => x.SubtitleStreams,
                    x => x.MapFrom(y => y.MediaStreams.Where(z => z.Type == MediaStreamType.Subtitle)));
        }




        public static int? MapInt(string input)
        {
            if (int.TryParse(input, out var tmdbValue))
            {
                return tmdbValue;
            }

            return null;
        }

    }
}
