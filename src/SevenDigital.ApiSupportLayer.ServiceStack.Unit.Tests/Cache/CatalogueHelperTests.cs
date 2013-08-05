using NUnit.Framework;
using SevenDigital.ApiSupportLayer.ServiceStack.Catalogue;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Cache
{
	[TestFixture]
	public class CatalogueHelperTests
	{
		[Test]
		public void TestName()
		{
			var expectedCountryListAndOrder = new[] {"GB", "US", "DE", "FR"};
			Assert.That(CatalogueHelper.CountriesToCheckInCatalogue, Is.EqualTo(expectedCountryListAndOrder));
		}
	}
}