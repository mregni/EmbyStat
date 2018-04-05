using System;
using AutoMapper;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Common.Tasks;
using EmbyStat.Controllers.Configuration;
using EmbyStat.Controllers.Emby;
using EmbyStat.Controllers.Plugin;
using EmbyStat.Controllers.Tasks;
using EmbyStat.Repositories.EmbyServerInfo;
using EmbyStat.Services.Emby.Models;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.System;

namespace EmbyStat.Controllers.Helpers
{
    public class MapProfiles : Profile
    {
	    public MapProfiles()
	    { 
			//Controllers
		    CreateMap<Repositories.Config.Configuration, ConfigurationViewModel>().ReverseMap();
		    CreateMap<EmbyUdpBroadcast, EmbyUdpBroadcastViewModel>().ReverseMap();
		    CreateMap<EmbyLogin, EmbyLoginViewModel>().ReverseMap();
		    CreateMap<EmbyToken, EmbyTokenViewModel>().ReverseMap();
		    CreateMap<PluginInfo, EmbyPluginViewModel>().ReverseMap();
		    CreateMap<ServerInfo, ServerInfoViewModel>().ReverseMap();
		    CreateMap<Repositories.EmbyDrive.Drives, DriveViewModel>()
				.ReverseMap()
			    .ForMember(x => x.Id, y => y.Ignore());

	        CreateMap<TaskInfo, TaskInfoViewModel>();
	        CreateMap<TaskResult, TaskResultViewModel>();
	        CreateMap<TaskTriggerInfo, TaskTriggerInfoViewModel>();
	        CreateMap<TaskStatus, TaskStatusViewModel>();

			//EmbyResponses
			CreateMap<SystemInfo, ServerInfo>()
			    .ForMember(x => x.Id, y => Guid.NewGuid())
			    .ReverseMap()
			    .ForMember(x => x.CompletedInstallations, y => y.Ignore())
			    .ForMember(x => x.CompletedInstallations, y => y.Ignore())
			    .ForMember(x => x.FailedPluginAssemblies, y => y.Ignore());

		    CreateMap<Drive, Repositories.EmbyDrive.Drives>()
			    .ForMember(x => x.Id, y => y.Ignore())
			    .ReverseMap();
	    }
    }
}
