using NUnit.Framework;
using SevenDigital.ApiInt.ServiceStack.Catalogue;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Cache
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