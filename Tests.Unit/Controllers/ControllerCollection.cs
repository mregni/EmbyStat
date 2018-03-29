using Xunit;

namespace Tests.Unit.Controllers
{
	[CollectionDefinition("Mapper collection")]
	public class ControllerCollection : ICollectionFixture<ControllerFixture>
	{
    }
}
