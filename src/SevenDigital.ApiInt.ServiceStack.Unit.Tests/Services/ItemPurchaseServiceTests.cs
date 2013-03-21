using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;
using SevenDigital.ApiInt.ServiceStack.Services;
using SevenDigital.ApiInt.ServiceStack.Unit.Tests.TestData;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class ProductCollaterTests
	{
		private ICatalogue _catalogue;

		[SetUp]
		public void SetUp()
		{
			_catalogue = MockRepository.GenerateStub<ICatalogue>();
			_catalogue.Stub(x => x.GetARelease(null, 0)).IgnoreArguments().Return(TestRelease.FleetFoxes);
			_catalogue.Stub(x => x.GetAReleaseTracks(null, 0)).IgnoreArguments().Return(new List<Track>());
		}

		[Test]
		public void TestName()
		{
			
		}

		public static IFluentApi<Release> GetStubbedReleaseApi(Release releaseToReturn)
		{
			var apiRelease = MockRepository.GenerateStub<IFluentApi<Release>>();
			apiRelease.Stub(x => x.WithParameter("", "")).IgnoreArguments().Return(apiRelease);
			apiRelease.Stub(x => x.ForReleaseId(0)).IgnoreArguments().Return(apiRelease);
			apiRelease.Stub(x => x.Please()).Return(releaseToReturn);
			return apiRelease;
		}

		public static IFluentApi<ReleaseTracks> GetStubbedReleaseTracksApi(Release releaseToReturn)
		{
			var apiRelease = MockRepository.GenerateStub<IFluentApi<ReleaseTracks>>();
			apiRelease.Stub(x => x.WithParameter("", "")).IgnoreArguments().Return(apiRelease);
			apiRelease.Stub(x => x.ForReleaseId(0)).IgnoreArguments().Return(apiRelease);
			apiRelease.Stub(x => x.Please()).Return(new ReleaseTracks { Tracks = new List<Track>() });
			return apiRelease;
		}
	}

	[TestFixture]
	public class ItemPurchaseServiceTests
	{
		private ICatalogue _catalogue;

		private IProductCollater _productCollater;

		[SetUp]
		public void SetUp()
		{
			_productCollater = MockRepository.GenerateStub<IProductCollater>();

			_catalogue = MockRepository.GenerateStub<ICatalogue>();
			_catalogue.Stub(x => x.GetARelease(null, 0)).IgnoreArguments().Return(TestRelease.FleetFoxes);
			_catalogue.Stub(x => x.GetAReleaseTracks(null, 0)).IgnoreArguments().Return(new List<Track>());
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
			Assert.That(((HttpResult)o).Response, Is.TypeOf<BuyItNowResponse<ReleaseAndTracks>>());
		}

		[Test]
		public void Happy_path_track()
		{
			_catalogue.Stub(x => x.GetATrack(null, 0)).IgnoreArguments().Return(TestTrack.SunItRises);
			
			var releaseService = new ItemPurchaseService(_productCollater);
			var releaseRequest = new ItemRequest { CountryCode = "GB", Id = 12345, Type = PurchaseType.track };
			var requestContext = MockRepository.GenerateStub<IRequestContext>();
			
			releaseService.RequestContext = requestContext;

			var o = releaseService.Get(releaseRequest);

			Assert.That(o, Is.TypeOf<HttpResult>());
			Assert.That(((HttpResult)o).Response, Is.TypeOf<BuyItNowResponse<ReleaseAndTracks>>());
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
	}
}