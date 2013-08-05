using NUnit.Framework;
using SevenDigital.ApiSupportLayer.ServiceStack.Model;
using SevenDigital.ApiSupportLayer.ServiceStack.Services;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Services
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
