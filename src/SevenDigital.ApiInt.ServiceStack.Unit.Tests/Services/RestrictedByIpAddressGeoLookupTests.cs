using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.Territories;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.ServiceStack.Services;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class RestrictedByIpAddressGeoLookupTests
	{
		[Test]
		public void returns_correct_message()
		{
			var geoLookupApi = MockRepository.GenerateStub<IFluentApi<GeoIpLookup>>();
			geoLookupApi.Stub(x => x.WithIpAddress("")).IgnoreArguments().Return(geoLookupApi);
			geoLookupApi.Stub(x => x.Please()).Return(new GeoIpLookup { CountryCode = "US", IpAddress = "86.131.235.233" });

			var countries = new List<Country>
			{
				new Country { Code = "GB", Url = "www.7digital.com", Description = "Great Britain"},
				new Country { Code = "US", Url = "us.7digital.com", Description = "the USA"},
			};
			var countryApi = MockRepository.GenerateStub<IFluentApi<Countries>>();
			countryApi.Stub(x => x.Please()).Return(new Countries()
			{
				CountryItems = countries
			});

			var restrictedByIpAddressGeoLookup = new GeoLookup(geoLookupApi, new ShopUrlService(countryApi));

			var restrictionMessage = restrictedByIpAddressGeoLookup.RestrictionMessage("GB", "");

			Assert.That(restrictionMessage, Is.EqualTo("&lt;p&gt;Sorry, but in accordance with our contractual obligations with the labels, you can only purchase from www.7digital.com if you live in Great Britain.&lt;/p&gt;&lt;p&gt;Please visit your local store at us.7digital.com to find an alternative version. If you’ve tried to access www.7digital.com from inside Great Britain, please get in touch with our Customer Support Team who will resolve the problem for you.&lt;/p&gt;&lt;p&gt;Thanks for your understanding!&lt;/p&gt;"));

		}
	}
}