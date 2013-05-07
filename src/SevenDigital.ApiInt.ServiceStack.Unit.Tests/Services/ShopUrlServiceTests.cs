using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.Territories;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.ServiceStack.Services;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class ShopUrlServiceTests
	{
		private IFluentApi<Countries> _mockApi;

		[SetUp]
		public void SetUp()
		{
			_mockApi = MockRepository.GenerateStub<IFluentApi<Countries>>();
			var countries = new List<Country>
			{
				new Country { Code = "GB", Url = "www.7digital.com"},
				new Country { Code = "DE", Url = "de.7digital.com" },
				new Country { Code = "AU", Url = "www.zdigital.com.au" }
			};
			_mockApi.Stub(x => x.Please()).Return(new Countries()
			{
				CountryItems = countries
			});
		}

		[Test]
		[TestCase("GB", "www.7digital.com")]
		[TestCase("DE", "de.7digital.com")]
		[TestCase("AU", "www.zdigital.com.au")]
		public void _then_I_should_get_the_correct_data_back(string countryCode, string expectedUrl)
		{
			var shopUrlService = new ShopUrlService(_mockApi);

			var request = new ShopUrl {CountryCode = countryCode};

			var response = shopUrlService.Get(request);

			Assert.That(response.Headers["Location"], Is.EqualTo(expectedUrl));
		}

		[Test]
		[TestCaseSource("ListOfGenericEuroCountries")]
		public void _then_I_should_get_the_correct_generic_url_for_other_european_countries(string countryCode)
		{
			var shopUrlService = new ShopUrlService(_mockApi);
			const string expectedUrl = "eu.7digital.com";

			var request = new ShopUrl { CountryCode = countryCode };

			var response = shopUrlService.Get(request);

			Assert.That(response.Headers["Location"], Is.EqualTo(expectedUrl));
		}

		[Test]
		[TestCase("ea")]
		[TestCase("dd")]
		public void _otherwise_it_should_return_correct_url_for_all_others(string countryCode)
		{
			var shopUrlService = new ShopUrlService(_mockApi);
			const string expectedUrl = "xw.7digital.com";

			var request = new ShopUrl { CountryCode = countryCode };

			var response = shopUrlService.Get(request);

			Assert.That(response.Headers["Location"], Is.EqualTo(expectedUrl));
		}

		[Test]
		public void _ting()
		{
			var shopUrlService = new ShopUrlService(_mockApi);

			var request = new ShopUrl { CountryCode = "GB" };

			var response = shopUrlService.Get(request);

			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
		}

		private static IEnumerable<string> ListOfGenericEuroCountries()
		{
			return ShopUrlConstants.GenericEuroCountryCodes();
		} 
	}
}