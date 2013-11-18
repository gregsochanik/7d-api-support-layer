using NUnit.Framework;
using SevenDigital.ApiSupportLayer.Catalogue;

namespace SevenDigital.ApiSupportLayer.Unit.Tests.Catalogue
{
	[TestFixture]
	public class CatalogueHelperTests
	{
		[Test]
		public void TestName()
		{
			var actulCountries = CatalogueHelper.CountriesToCheckInCatalogue;
			var expectedCountries = new[] {"GB", "US", "DE", "FR"};

			Assert.That(actulCountries, Is.EqualTo(expectedCountries));
		}
	}
}
