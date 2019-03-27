using System;
using AutoMapper;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Controllers.About;
using EmbyStat.Controllers.Emby;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.Job;
using EmbyStat.Controllers.Log;
using EmbyStat.Controllers.Movie;
using EmbyStat.Controllers.Settings;
using EmbyStat.Controllers.Show;
using EmbyStat.Controllers.System;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Graph;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.System;

namespace EmbyStat.Controllers
{
    public class MapProfiles : Profile
    {
	    public MapProfiles()
	    { 
            //EmbyHttpResponses
            CreateMap<MediaBrowser.Model.Plugins.PluginInfo, PluginInfo>();

            //Controllers
            CreateMap<UserSettings, FullSettingsViewModel>().ForMember(x => x.Version, x => x.Ignore()).ReverseMap();
		    CreateMap<EmbyUdpBroadcast, EmbyUdpBroadcastViewModel>().ReverseMap();
		    CreateMap<EmbyLogin, EmbyLoginViewModel>().ReverseMap();
		    CreateMap<EmbyToken, EmbyTokenViewModel>().ReverseMap();
		    CreateMap<PluginInfo, EmbyPluginViewModel>();
		    CreateMap<ServerInfo, ServerInfoViewModel>();
            CreateMap<UpdateResult, UpdateResultViewModel>();

	        CreateMap<Common.Models.Entities.Job, JobViewModel>();
	        CreateMap<TimeSpanCard, TimeSpanCardViewModel>();
	        CreateMap( typeof(Card<>), typeof(CardViewModel<>));
	        CreateMap<MoviePoster, MoviePosterViewModel>();
	        CreateMap<ShowPoster, ShowPosterViewModel>();
            CreateMap<PersonPoster, PersonPosterViewModel>();
            CreateMap<MovieStats, MovieStatsViewModel>();
	        CreateMap<PersonStats, PersonStatsViewModel>();
            CreateMap<Collection, CollectionViewModel>();
	        CreateMap<MovieDuplicate, MovieDuplicateViewModel>();
	        CreateMap<MovieDuplicateItem, MovieDuplicateItemViewModel>();
	        CreateMap<SimpleGraphValue, SimpleGraphValueViewModel>();
	        CreateMap<Graph<SimpleGraphValue>, GraphViewModel<SimpleGraphValue>>();
	        CreateMap<MovieGraphs, MovieGraphsViewModel>();
	        CreateMap<ShowGraphs, ShowGraphsViewModel>();
            CreateMap<ShortMovie, ShortMovieViewModel>();
	        CreateMap<SuspiciousTables, SuspiciousTablesViewModel>();
	        CreateMap<ShowStat, ShowStatViewModel>();
	        CreateMap<ShowCollectionRow, ShowCollectionRowViewModel>();
	        CreateMap<LogFile, LogFileViewModel>();
	        CreateMap<Services.Models.About.About, AboutViewModel>();
	        CreateMap<SuspiciousMovie, SuspiciousMovieViewModel>();
            CreateMap<User, UserIdViewModel>();
            CreateMap<User, EmbyUserOverviewViewModel>();
            CreateMap<User, EmbyUserFullViewModel>();
            CreateMap<UserAccessSchedule, UserAccessScheduleViewModel>();
            CreateMap<UserMediaView, UserMediaViewViewModel>();
            //EmbyResponses
            CreateMap<SystemInfo, ServerInfo>()
			    .ForMember(x => x.Id, y => Guid.NewGuid())
			    .ReverseMap()
			    .ForMember(x => x.CompletedInstallations, y => y.Ignore());
	    }
    }
}
