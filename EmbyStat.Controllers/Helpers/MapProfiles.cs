using AutoMapper;
using EmbyStat.Controllers.Configuration;
using EmbyStat.Controllers.Emby;
using EmbyStat.Services.Emby.Models;

namespace EmbyStat.Controllers.Helpers
{
    public class MapProfiles : Profile
    {
	    public MapProfiles()
	    {
		    CreateMap<Repositories.Config.Configuration, ConfigurationViewModel>().ReverseMap();
		    CreateMap<EmbyUdpBroadcast, EmbyUdpBroadcastViewModel>().ReverseMap();
		    CreateMap<EmbyLogin, EmbyLoginViewModel>().ReverseMap();
		    CreateMap<EmbyToken, EmbyTokenViewModel>().ReverseMap();
	    }
    }
}
