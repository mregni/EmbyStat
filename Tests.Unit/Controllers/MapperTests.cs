using AutoMapper;
using Xunit;

namespace Tests.Unit.Controllers
{
	[Collection("Mapper collection")]
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
