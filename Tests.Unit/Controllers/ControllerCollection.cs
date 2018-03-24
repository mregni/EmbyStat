using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests.Unit.Controllers
{
	[CollectionDefinition("Controller collection")]
	public class ControllerCollection : ICollectionFixture<ControllerFixture>
	{
    }
}
