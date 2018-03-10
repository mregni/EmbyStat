using AutoMapper;
using EmbyStat.Controllers.Configuration;
using EmbyStat.Services.Config.Models;

namespace EmbyStat.Controllers.Helpers
{
    public class MapProfiles : Profile
    {
	    public MapProfiles()
	    {
		    CreateMap<Repositories.Config.Configuration, ConfigurationViewModel>().ReverseMap();
		    CreateMap<EmbyUdpBroadcast, EmbyUdpBroadcastViewModel>().ReverseMap();
	    }
    }
}
