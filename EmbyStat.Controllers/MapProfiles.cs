using System;
using AutoMapper;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Common.Models.Show;
using EmbyStat.Controllers.About;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.Job;
using EmbyStat.Controllers.Log;
using EmbyStat.Controllers.MediaServer;
using EmbyStat.Controllers.Movie;
using EmbyStat.Controllers.Settings;
using EmbyStat.Controllers.Show;
using EmbyStat.Controllers.System;
using EmbyStat.Services.Models.Charts;
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
                .ForMember(x => x.Version, x => x.Ignore());
            CreateMap<MediaServerSettings, FullSettingsViewModel.EmbySettingsViewModel>().ReverseMap();
            CreateMap<TvdbSettings, FullSettingsViewModel.TvdbSettingsViewModel>().ReverseMap();
            CreateMap<Language, LanguageViewModel>();
		    CreateMap<EmbyUdpBroadcast, UdpBroadcastViewModel>().ReverseMap();
            CreateMap<PluginInfo, PluginViewModel>();
		    CreateMap<ServerInfo, ServerInfoViewModel>();
            CreateMap<UpdateResult, UpdateResultViewModel>();
	        CreateMap<Common.Models.Entities.Job, JobViewModel>();
	        CreateMap<TimeSpanCard, TimeSpanCardViewModel>();
	        CreateMap( typeof(Card<>), typeof(CardViewModel<>));
	        CreateMap<MoviePoster, MoviePosterViewModel>();
	        CreateMap<ShowPoster, ShowPosterViewModel>();
            CreateMap<PersonPoster, PersonPosterViewModel>();
            CreateMap<MovieStatistics, MovieStatisticsViewModel>();
            CreateMap<ShowStatistics, ShowStatisticsViewModel>();
            CreateMap<MovieGeneral, MovieGeneralViewModel>();
	        CreateMap<PersonStats, PersonStatsViewModel>();
            CreateMap<Library, LibraryViewModel>();
	        CreateMap<MovieDuplicate, MovieDuplicateViewModel>();
	        CreateMap<MovieDuplicateItem, MovieDuplicateItemViewModel>();
	        CreateMap<Chart, ChartViewModel>();
	        CreateMap<MovieCharts, MovieChartsViewModel>();
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
            CreateMap<UserAccessSchedule, UserAccessScheduleViewModel>();
            CreateMap<UserMediaView, UserMediaViewViewModel>();
            CreateMap<VirtualSeason, VirtualSeasonViewModel>();
            CreateMap<VirtualEpisode, VirtualEpisodeViewModel>();
            CreateMap<SystemInfo, ServerInfo>()
			    .ForMember(x => x.Id, y => Guid.NewGuid())
			    .ReverseMap()
			    .ForMember(x => x.CompletedInstallations, y => y.Ignore());
            CreateMap(typeof(ListContainer<>), typeof(ListContainer<>));
        }
    }
}
