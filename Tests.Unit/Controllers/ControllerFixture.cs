using System;
using AutoMapper;
using EmbyStat.Controllers;

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
