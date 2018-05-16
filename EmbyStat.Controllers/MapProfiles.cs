using System;
using AutoMapper;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Common.Models;
using EmbyStat.Common.Tasks;
using EmbyStat.Controllers.ViewModels.Configuration;
using EmbyStat.Controllers.ViewModels.Emby;
using EmbyStat.Controllers.ViewModels.Graph;
using EmbyStat.Controllers.ViewModels.Movie;
using EmbyStat.Controllers.ViewModels.Server;
using EmbyStat.Controllers.ViewModels.Show;
using EmbyStat.Controllers.ViewModels.Stat;
using EmbyStat.Controllers.ViewModels.Task;
using EmbyStat.Services.Models.Stat;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Graph;
using EmbyStat.Services.Models.Show;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.System;

namespace EmbyStat.Controllers.Helpers
{
    public class MapProfiles : Profile
    {
	    public MapProfiles()
	    { 
			//Controllers
		    CreateMap<Configuration, ConfigurationViewModel>().ReverseMap();
		    CreateMap<EmbyUdpBroadcast, EmbyUdpBroadcastViewModel>().ReverseMap();
		    CreateMap<EmbyLogin, EmbyLoginViewModel>().ReverseMap();
		    CreateMap<EmbyToken, EmbyTokenViewModel>().ReverseMap();
		    CreateMap<PluginInfo, EmbyPluginViewModel>();
		    CreateMap<ServerInfo, ServerInfoViewModel>();
		    CreateMap<Drives, DriveViewModel>();

	        CreateMap<TaskInfo, TaskInfoViewModel>();
	        CreateMap<TaskResult, TaskResultViewModel>();
	        CreateMap<TaskTriggerInfo, TaskTriggerInfoViewModel>();
	        CreateMap<TimeSpanCard, TimeSpanCardViewModel>();
	        CreateMap<Card, CardViewModel>();
	        CreateMap<MoviePoster, MoviePosterViewModel>();
	        CreateMap<ShowPoster, ShowPosterViewModel>();
            CreateMap<PersonPoster, PersonPosterViewModel>();
            CreateMap<MovieStats, MovieStatsViewModel>();
	        CreateMap<MoviePersonStats, MoviePersonStatsViewModel>();
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

            //EmbyResponses
            CreateMap<SystemInfo, ServerInfo>()
			    .ForMember(x => x.Id, y => Guid.NewGuid())
			    .ReverseMap()
			    .ForMember(x => x.CompletedInstallations, y => y.Ignore());

		    CreateMap<Drive, Drives>()
			    .ForMember(x => x.Id, y => y.Ignore())
			    .ReverseMap();
	    }
    }
}
