using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface.Testing;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Wrapper.Http;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Catalogue;
using SevenDigital.ApiInt.ServiceStack.Model;
using SevenDigital.ApiInt.ServiceStack.Services;
using SevenDigital.ApiInt.TestData;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class ItemPurchaseServiceTests
	{
		private IProductCollater _productCollater;
		private MockRequestContext _requestContext;
		private IGeoLookup _geoLookup;
		private IGeoSettings _geoSettings;

		[SetUp]
		public void SetUp()
		{
			_productCollater = MockRepository.GenerateStub<IProductCollater>();
			_geoLookup = MockRepository.GenerateStub<IGeoLookup>();
			_geoSettings = MockRepository.GenerateStub<IGeoSettings>();
			_requestContext = ContextHelper.LoggedInContext();
		}

		[Test]
		public void Happy_path_release()
		{
			var releaseService = new ItemPurchaseService(_productCollater, _geoLookup, _geoSettings)
			{
				RequestContext = _requestContext
			};

			var releaseRequest = new ItemRequest{ CountryCode = "GB", Id = 12345, Type = PurchaseType.release};

			var o = releaseService.Get(releaseRequest);

			Assert.That(o, Is.TypeOf<HttpResult>());
			Assert.That(o.Response, Is.TypeOf<BuyItNowResponse<ReleaseAndTracks>>());

			_productCollater.AssertWasCalled(x=>x.UsingReleaseId("GB", 12345));
		}

		[Test]
		public void Happy_path_track()
		{
			var releaseService = new ItemPurchaseService(_productCollater, _geoLookup, _geoSettings)
			{
				RequestContext = _requestContext
			};

			var releaseRequest = new ItemRequest { CountryCode = "GB", Id = 12345, Type = PurchaseType.track };

			var o = releaseService.Get(releaseRequest);

			Assert.That(o, Is.TypeOf<HttpResult>());
			Assert.That(o.Response, Is.TypeOf<BuyItNowResponse<ReleaseAndTracks>>());

			_productCollater.AssertWasCalled(x=>x.UsingTrackId("GB", 12345));
		}

		[Test]
		public void Throws_error_if_no_releaseId_specified()
		{
			var releaseService = new ItemPurchaseService(_productCollater, _geoLookup, _geoSettings) { RequestContext = new MockRequestContext() }; ;
			var releaseRequest = new ItemRequest();

			var argumentNullException = Assert.Throws<ArgumentNullException>(() => releaseService.Get(releaseRequest));

			Assert.That(argumentNullException.ParamName, Is.EqualTo("request"));
			Assert.That(argumentNullException.Message, Is.EqualTo("You must specify an Id\r\nParameter name: request"));
		}

		[Test]
		public void Throws_error_if_territory_restriction_applies()
		{
			var geoLookup = MockRepository.GenerateStub<IGeoLookup>();
			geoLookup.Stub(x => x.IsRestricted("", "")).IgnoreArguments().Return(true);
			geoLookup.Stub(x => x.RestrictionMessage("", "")).IgnoreArguments().Return("RestrictionMessage!");

			var geoSettings = MockRepository.GenerateStub<IGeoSettings>();
			geoSettings.Stub(x => x.IsTiedToIpAddress()).Return(true);

			var releaseService = new ItemPurchaseService(_productCollater, geoLookup, geoSettings) { RequestContext = new MockRequestContext() }; ;
			var releaseRequest = new ItemRequest { CountryCode = "GB", Id = 12345, Type = PurchaseType.track };

			var httpError = Assert.Throws<HttpError>(() => releaseService.Get(releaseRequest));

			Assert.That(httpError.Message, Is.EqualTo("RestrictionMessage!"));
			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
		}

		[Test]
		public void Throws_error_if_api_throws_InvalidResourceException()
		{
			var productCollaterThatThrowsInvalidResourceException = MockRepository.GenerateStub<IProductCollater>();
			productCollaterThatThrowsInvalidResourceException.Stub(x => x.UsingTrackId("GB", 1234)).IgnoreArguments().Throw(new InvalidResourceException("blah", new Response(HttpStatusCode.NotFound, "NotFound"), ErrorCode.ResourceNotFound ));

			var releaseService = new ItemPurchaseService(productCollaterThatThrowsInvalidResourceException, _geoLookup, _geoSettings) { RequestContext = new MockRequestContext() };
			var releaseRequest = new ItemRequest{CountryCode = "GB", Id = 1234, Type = PurchaseType.track};

			var exception = Assert.Throws<HttpError>(() => releaseService.Get(releaseRequest));

			Assert.That(exception.Message, Is.EqualTo("Not found"));
			Assert.That(exception.ErrorCode, Is.EqualTo(ErrorCode.ResourceNotFound.ToString()));
			Assert.That(exception.Response, Is.TypeOf<ItemRequest>());
			Assert.That(((ItemRequest)exception.Response).CountryCode, Is.EqualTo("GB"));
			Assert.That(((ItemRequest)exception.Response).Id, Is.EqualTo(1234));
		}

		[Test]
		public void If_bundle_track_fire_Get_with_release()
		{
			var productCollater = MockRepository.GenerateStub<IProductCollater>();
			productCollater.Stub(x => x.UsingTrackId(null, 0)).IgnoreArguments().Return(new ReleaseAndTracks
			{
				Type = PurchaseType.track,
				Release = TestRelease.FleetFoxes,
				Tracks = new List<Track>
				{
					TestTrack.BundleTrack
				}
			});
			productCollater.Stub(x => x.UsingReleaseId(null, 0)).IgnoreArguments().Return(new ReleaseAndTracks
			{
				Type = PurchaseType.release,
				Release = TestRelease.FleetFoxes,
				Tracks = new List<Track>
				{
					TestTrack.BundleTrack
				}
			});
			var releaseService = new ItemPurchaseService(productCollater, _geoLookup, _geoSettings)
			{
				RequestContext = _requestContext
			};

			var releaseRequest = new ItemRequest { Id=1, Type=PurchaseType.track };

			var httpResult = releaseService.Get(releaseRequest);

			Assert.That(httpResult.Response, Is.Not.Null);
		}
	}
}