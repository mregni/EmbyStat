using AutoMapper;
using EmbyStat.Controllers.Configuration;

namespace EmbyStat.Controllers.Helpers
{
    public class MapProfiles : Profile
    {
	    public MapProfiles()
	    {
		    CreateMap<Repositories.Config.Configuration, ConfigurationViewModel>();
	    }
    }
}
