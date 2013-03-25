﻿using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Schema.ReleaseEndpoint;
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

		[SetUp]
		public void SetUp()
		{
			_productCollater = MockRepository.GenerateStub<IProductCollater>();
		}

		[Test]
		public void Happy_path_release()
		{
			var releaseService = new ItemPurchaseService(_productCollater);
			var releaseRequest = new ItemRequest{ CountryCode = "GB", Id = 12345, Type = PurchaseType.release};
			var requestContext = MockRepository.GenerateStub<IRequestContext>();

			releaseService.RequestContext = requestContext;

			var o = releaseService.Get(releaseRequest);

			Assert.That(o, Is.TypeOf<HttpResult>());
			Assert.That(o.Response, Is.TypeOf<BuyItNowResponse<ReleaseAndTracks>>());

			_productCollater.AssertWasCalled(x=>x.UsingReleaseId("GB", 12345));
		}

		[Test]
		public void Happy_path_track()
		{
			var releaseService = new ItemPurchaseService(_productCollater);
			var releaseRequest = new ItemRequest { CountryCode = "GB", Id = 12345, Type = PurchaseType.track };
			var requestContext = MockRepository.GenerateStub<IRequestContext>();
			
			releaseService.RequestContext = requestContext;

			var o = releaseService.Get(releaseRequest);

			Assert.That(o, Is.TypeOf<HttpResult>());
			Assert.That(o.Response, Is.TypeOf<BuyItNowResponse<ReleaseAndTracks>>());

			_productCollater.AssertWasCalled(x=>x.UsingTrackId("GB", 12345));
		}

		[Test]
		public void Throws_error_if_no_releaseId_specified()
		{
			var releaseService = new ItemPurchaseService(_productCollater);
			var releaseRequest = new ItemRequest();

			var argumentNullException = Assert.Throws<ArgumentNullException>(() => releaseService.Get(releaseRequest));

			Assert.That(argumentNullException.ParamName, Is.EqualTo("request"));
			Assert.That(argumentNullException.Message, Is.EqualTo("You must specify an Id\r\nParameter name: request"));
		}

		[Test]
		public void Throws_error_if_api_throws_InvalidResourceException()
		{
			var productCollaterThatThrowsInvalidResourceException = MockRepository.GenerateStub<IProductCollater>();
			productCollaterThatThrowsInvalidResourceException.Stub(x => x.UsingTrackId("GB", 1234)).IgnoreArguments().Throw(new InvalidResourceException("blah", new Response(HttpStatusCode.NotFound, "NotFound"), ErrorCode.ResourceNotFound ));

			var releaseService = new ItemPurchaseService(productCollaterThatThrowsInvalidResourceException);
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
			var releaseService = new ItemPurchaseService(productCollater);


			var requestContext = MockRepository.GenerateStub<IRequestContext>();

			releaseService.RequestContext = requestContext;

			var releaseRequest = new ItemRequest { Id=1, Type=PurchaseType.track };

			var httpResult = releaseService.Get(releaseRequest);

			Assert.That(httpResult.Response, Is.Not.Null);
		}
	}
}