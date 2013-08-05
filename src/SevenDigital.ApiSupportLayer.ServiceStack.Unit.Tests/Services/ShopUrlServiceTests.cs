using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.Territories;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiSupportLayer.ServiceStack.Services;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Services
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
		public void _then_I_should_get_the_correct_data_back_for_cases_that_exist_in_the_countries_api(string countryCode, string expectedUrl)
		{
			var shopUrlService = new ShopUrlService(_mockApi);

			var request = new ShopUrl {CountryCode = countryCode};

			var response = shopUrlService.Get(request);

			Assert.That(response.Headers["Location"], Is.EqualTo(expectedUrl));
		}

		[Test]
		[TestCaseSource("ListOfGenericEuroCountries")]
		public void _then_I_should_get_the_correct_generic_url_for_other_european_countries_that_dont_appear_in_countries_api(string countryCode)
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
		public void _otherwise_it_should_return_correct_worldwide_url_for_all_others(string countryCode)
		{
			var shopUrlService = new ShopUrlService(_mockApi);
			const string expectedUrl = "xw.7digital.com";

			var request = new ShopUrl { CountryCode = countryCode };

			var response = shopUrlService.Get(request);

			Assert.That(response.Headers["Location"], Is.EqualTo(expectedUrl));
		}

		[Test]
		public void _it_should_perform_a_redirect()
		{
			var shopUrlService = new ShopUrlService(_mockApi);

			var request = new ShopUrl { CountryCode = "GB" };

			var response = shopUrlService.Get(request);

			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
		}

		[Test]
		public void _it_should_add_the_urlPath_arg_to_the_end_of_the_redirect()
		{
			var shopUrlService = new ShopUrlService(_mockApi);
			const string expectedUrl = "www.7digital.com";
			var request = new ShopUrl { CountryCode = "GB", UrlPath = "boom/bang" };

			var response = shopUrlService.Get(request);

			Assert.That(response.Headers["Location"], Is.EqualTo(expectedUrl + "/boom/bang"));
		}

		private static IEnumerable<string> ListOfGenericEuroCountries()
		{
			return ShopUrlConstants.GenericEuroCountryCodes();
		} 
	}
}