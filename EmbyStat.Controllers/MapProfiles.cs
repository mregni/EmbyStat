using System;
using AutoMapper;
using EmbyStat.Clients.EmbyClient.Model;
using EmbyStat.Clients.Github.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers.ViewModels.About;
using EmbyStat.Controllers.ViewModels.Configuration;
using EmbyStat.Controllers.ViewModels.Emby;
using EmbyStat.Controllers.ViewModels.Graph;
using EmbyStat.Controllers.ViewModels.Logs;
using EmbyStat.Controllers.ViewModels.Movie;
using EmbyStat.Controllers.ViewModels.Server;
using EmbyStat.Controllers.ViewModels.Show;
using EmbyStat.Controllers.ViewModels.Stat;
using EmbyStat.Controllers.ViewModels.Task;
using EmbyStat.Controllers.ViewModels.Update;
using EmbyStat.Services.Models.About;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Graph;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.System;

namespace EmbyStat.Controllers
{
    public class MapProfiles : Profile
    {
	    public MapProfiles()
	    { 
			//Controllers
		    CreateMap<Configuration, ConfigurationViewModel>().ReverseMap().ForMember(x => x.Version, x => x.Ignore());
		    CreateMap<EmbyUdpBroadcast, EmbyUdpBroadcastViewModel>().ReverseMap();
		    CreateMap<EmbyLogin, EmbyLoginViewModel>().ReverseMap();
		    CreateMap<EmbyToken, EmbyTokenViewModel>().ReverseMap();
		    CreateMap<PluginInfo, EmbyPluginViewModel>();
		    CreateMap<ServerInfo, ServerInfoViewModel>();
		    CreateMap<Drive, DriveViewModel>();
            CreateMap<UpdateResult, UpdateResultViewModel>();

	        CreateMap<Job, JobViewModel>();
	        CreateMap<TimeSpanCard, TimeSpanCardViewModel>();
	        CreateMap<Card, CardViewModel>();
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
	        CreateMap<About, AboutViewModel>();
	        CreateMap<SuspiciousMovie, SuspiciousMovieViewModel>();

            //EmbyResponses
            CreateMap<SystemInfo, ServerInfo>()
			    .ForMember(x => x.Id, y => Guid.NewGuid())
			    .ReverseMap()
			    .ForMember(x => x.CompletedInstallations, y => y.Ignore());
	    }
    }
}
