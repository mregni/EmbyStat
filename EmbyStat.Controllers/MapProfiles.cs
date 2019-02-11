using System;
using AutoMapper;
using EmbyStat.Clients.EmbyClient.Model;
using EmbyStat.Clients.Github.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Controllers.About;
using EmbyStat.Controllers.Emby;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.Job;
using EmbyStat.Controllers.Log;
using EmbyStat.Controllers.Movie;
using EmbyStat.Controllers.Plugin;
using EmbyStat.Controllers.Settings;
using EmbyStat.Controllers.Show;
using EmbyStat.Controllers.System;
using EmbyStat.Services.Models.About;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Graph;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.System;
using PluginInfo = EmbyStat.Common.Models.Entities.PluginInfo;

namespace EmbyStat.Controllers
{
    public class MapProfiles : Profile
    {
	    public MapProfiles()
	    { 
            //EmbyHttpResponses
            CreateMap<MediaBrowser.Model.Plugins.PluginInfo, PluginInfo>();
            CreateMap<FileSystemEntryInfo, Drive>();

            //Controllers
            CreateMap<UserSettings, FullSettingsViewModel>().ForMember(x => x.Version, x => x.Ignore()).ReverseMap();
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
