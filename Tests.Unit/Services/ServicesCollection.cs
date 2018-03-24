using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests.Unit.Services
{
	[CollectionDefinition("Services collection")]
	public class ServicesCollection : ICollectionFixture<ServicesFixture>
    {
    }
}
