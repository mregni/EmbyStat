using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using EmbyStat.Controllers.Helpers;
using Xunit;

namespace Tests.Unit
{
	[Collection("Controller collection")]
	public class MapperTests
    {
		public MapperTests()
	    {

	    }

		[Fact]
	    public void AreMappingsCorrect()
	    {
			Mapper.AssertConfigurationIsValid();
		}
    }
}
