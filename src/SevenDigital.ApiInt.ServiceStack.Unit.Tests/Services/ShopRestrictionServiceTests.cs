﻿using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.Territories;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.ServiceStack.Services;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class ShopRestrictionServiceTests
	{
		private IFluentApi<GeoIpLookup> _ipLookupApi;

		[SetUp]
		public void SetUp()
		{
			_ipLookupApi = MockRepository.GenerateStub<IFluentApi<GeoIpLookup>>();
			_ipLookupApi.Stub(x => x.WithIpAddress("")).IgnoreArguments().Return(_ipLookupApi);
			_ipLookupApi.Stub(x => x.Please()).Return(new GeoIpLookup{CountryCode = "GB", IpAddress = "86.131.235.233"});
		}

		[Test]
		public void _restricts_if_selected_country_doesnt_match()
		{
			
			var shopRestrictionService = new ShopRestrictionService(_ipLookupApi);
			var shopRestriction = shopRestrictionService.Get(new ShopRestriction
			{
				IpAddress = "86.131.235.233",
				CountryCode = "AU"
			});

			Assert.That(shopRestriction.IsRestricted);
		}

		[Test]
		public void _doesnt_restrict_if_selected_country_doesnt_match()
		{
			var shopRestrictionService = new ShopRestrictionService(_ipLookupApi);
			var shopRestriction = shopRestrictionService.Get(new ShopRestriction
			{
				IpAddress = "86.131.235.233",
				CountryCode = "GB"
			});

			Assert.That(shopRestriction.IsRestricted, Is.False);
		}
	}
}