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
			var countriesToCheckInCatalogue = CatalogueHelper.CountriesToCheckInCatalogue;

			var expectedCountries = new[] {"GB", "US", "DE", "FR"};
			var enumerator = countriesToCheckInCatalogue.GetEnumerator();

			foreach (var expectedCountry in expectedCountries)
			{
				enumerator.MoveNext();
				Assert.That(enumerator.Current, Is.EqualTo(expectedCountry));
			}
		}
	}
}
