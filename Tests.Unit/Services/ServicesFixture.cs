using System;
using AutoMapper;
using EmbyStat.Controllers.Helpers;

namespace Tests.Unit.Services
{
    public class ServicesFixture : IDisposable
    {
	    public ServicesFixture()
	    {
			Mapper.Initialize(cfg => cfg.AddProfile<MapProfiles>());
		}

	    public void Dispose()
	    {
			Mapper.Reset();
	    }
    }
}
