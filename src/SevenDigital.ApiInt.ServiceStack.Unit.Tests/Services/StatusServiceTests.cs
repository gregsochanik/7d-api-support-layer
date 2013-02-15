using NUnit.Framework;
using SevenDigital.ApiInt.ServiceStack.Model;
using SevenDigital.ApiInt.ServiceStack.Services;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class StatusServiceTests
	{
		[Test]
		public void Returns_a_valid_server_time()
		{
			var statusService = new StatusService();
			var applicationState = new ApplicationState();
			var o = statusService.Get(applicationState);

			Assert.That(o, Is.TypeOf<ApplicationState>());
			Assert.That(((ApplicationState)o).ServerTime, Is.Not.Null);
		}
	}
}
