using Xunit;

namespace Tests.Unit.Services
{
	[CollectionDefinition("Services collection")]
	public class ServicesCollection : ICollectionFixture<ServicesFixture>
    {
    }
}
