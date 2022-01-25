using System;
using System.Linq;
using AutoMapper;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Common.Models.Show;
using EmbyStat.Common.SqLite;
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
using MediaBrowser.Model.System;
using LogFile = EmbyStat.Services.Models.Logs.LogFile;

namespace EmbyStat.Controllers
{
    public class MapProfiles : Profile
    {
	    public MapProfiles()
	    { 
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
	        CreateMap( typeof(Card<>), typeof(CardViewModel<>))
                .ForMember("Type", src => src.MapFrom((org, x) =>
                {
                    return ((Card<string>) org).Type switch
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

            CreateMap(typeof(Page<>), typeof(PageViewModel<>));
            CreateMap<MovieRow, MovieRowViewModel>()
                .ForMember(x => x.Genres, x => x.MapFrom(y => y.Genres.Select(z => z.Name)));
            CreateMap<TopCard, TopCardViewModel>();
            CreateMap<TopCardItem, TopCardItemViewModel>();
            CreateMap<LabelValuePair, LabelValuePairViewModel>();
            CreateMap<FilterValues, FilterValuesViewModel>();
            CreateMap<EmbyStatus, EmbyStatusViewModel>();
            CreateMap<ShowRow, ShowRowViewModel>();
            _ = CreateMap<Common.Models.Entities.Show, ShowDetailViewModel>()
                .ForMember(x => x.SizeInMb, x => x.MapFrom(y => y.Episodes.Sum(z => z.MediaSources.FirstOrDefault() != null ? z.MediaSources.First().SizeInMb : 0.0)))
                .ForMember(x => x.CollectedEpisodeCount, x => x.MapFrom(y => y.GetEpisodeCount(false, LocationType.Disk)))
                .ForMember(x => x.MissingEpisodes, x => x.MapFrom(y => y.GetMissingEpisodes()))
                .ForMember(x => x.SeasonCount, x => x.MapFrom(y => y.GetSeasonCount(false)))
                .ForMember(x => x.SpecialEpisodeCount, x => x.MapFrom(y => y.GetEpisodeCount(true, LocationType.Disk)));

            CreateMap<SqlMovie, MovieViewModel>()
                .ForMember(x => x.Genres, x => x.MapFrom(y => y.Genres.Select(z => z.Name)));
            CreateMap<SqlAudioStream, AudioStreamViewModel>();
            CreateMap<SqlMediaSource, MediaSourceViewModel>();
            CreateMap<SqlSubtitleStream, SubtitleStreamViewModel>();
            CreateMap<SqlVideoStream, VideoStreamViewModel>();
        }
    }
}
