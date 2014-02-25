using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Wrapper.Http;
using SevenDigital.Api.Wrapper.Responses;
using SevenDigital.ApiSupportLayer.GeoLocation;
using SevenDigital.ApiSupportLayer.Restrictions;

namespace SevenDigital.ApiSupportLayer.Unit.Tests.Restrictions
{
	[TestFixture]
	public class IpAddressRestrictorTests
	{
		[Test]
		public void Returns_no_restriction_if_allowed()
		{
			var geoLookup = MockRepository.GenerateStub<IGeoLookup>();
			var geoSettings = MockRepository.GenerateStub<IGeoSettings>();
			var ipAddressRestrictor = new IpAddressRestrictor(geoLookup, geoSettings);

			var keyValuePair = new KeyValuePair<string, string>("GB", "192.168.0.1");
			var assertRestriction = ipAddressRestrictor.AssertRestriction(keyValuePair);

			Assert.That(assertRestriction.Message, Is.Empty);
			Assert.That(assertRestriction.Type, Is.EqualTo(RestrictionType.NoRestriction));
		}

		[Test]
		public void Returns_territory_restriction_if_validIp_but_restricted_by_country()
		{
			var geoLookup = MockRepository.GenerateStub<IGeoLookup>();
			var geoSettings = MockRepository.GenerateStub<IGeoSettings>();

			geoSettings.Stub(x => x.IsTiedToIpAddress()).Return(true);
			geoLookup.Stub(x => x.IsRestricted("", "")).IgnoreArguments().Return(true);

			const string expectedMessage = "Test message";
			geoLookup.Stub(x => x.RestrictionMessage("", "")).IgnoreArguments().Return(expectedMessage);

			var ipAddressRestrictor = new IpAddressRestrictor(geoLookup, geoSettings);

			var keyValuePair = new KeyValuePair<string, string>("", "");
			var assertRestriction = ipAddressRestrictor.AssertRestriction(keyValuePair);

			Assert.That(assertRestriction.Message, Is.EqualTo(expectedMessage));
			Assert.That(assertRestriction.Type, Is.EqualTo(RestrictionType.TerritoryRestriction));
		}

		[Test]
		public void Returns_territory_restriction_invalid_ip_if_invalidIp_but_restricted_by_country()
		{
			var geoLookup = MockRepository.GenerateStub<IGeoLookup>();
			var geoSettings = MockRepository.GenerateStub<IGeoSettings>();

			geoSettings.Stub(x => x.IsTiedToIpAddress()).Return(true);
			geoLookup.Stub(x => x.IsRestricted("", "")).IgnoreArguments().Return(true);

			geoLookup.Stub(x => x.RestrictionMessage("", "")).IgnoreArguments().Throw(new InputParameterException("", new Response(HttpStatusCode.BadRequest, ""),ErrorCode.InvalidParameterValue ));

			var ipAddressRestrictor = new IpAddressRestrictor(geoLookup, geoSettings);

			var keyValuePair = new KeyValuePair<string, string>("", "");
			var assertRestriction = ipAddressRestrictor.AssertRestriction(keyValuePair);

			Assert.That(assertRestriction.Message, Is.EqualTo(""));
			Assert.That(assertRestriction.Type, Is.EqualTo(RestrictionType.TerritoryRestrictionInvalidIpAddress));
		}
	}
}
