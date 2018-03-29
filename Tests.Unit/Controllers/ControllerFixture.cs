using System;
using AutoMapper;
using EmbyStat.Controllers.Helpers;

namespace Tests.Unit.Controllers
{
    public class ControllerFixture : IDisposable
    {
	    public ControllerFixture()
	    {
			Mapper.Initialize(cfg => cfg.AddProfile<MapProfiles>());
		}
	    public void Dispose()
	    {
		    Mapper.Reset();
		}
    }
}
