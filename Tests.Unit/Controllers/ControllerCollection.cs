using Xunit;

namespace Tests.Unit.Controllers
{
	[CollectionDefinition("Controller collection")]
	public class ControllerCollection : ICollectionFixture<ControllerFixture>
	{
    }
}
