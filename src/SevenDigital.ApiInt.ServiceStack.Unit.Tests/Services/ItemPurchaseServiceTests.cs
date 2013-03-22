using System;
using System.Collections.Generic;
using System.Linq;
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
		[Test]
		public void Collating_a_release_returns_release_and_all_tracks()
		{
			const int expectedReleaseTrackCount = 5;

			var catalogue = MockRepository.GenerateStub<ICatalogue>();
			catalogue.Stub(x => x.GetARelease(null, 0)).IgnoreArguments().Return(TestRelease.FleetFoxes);
			catalogue.Stub(x => x.GetAReleaseTracks(null, 0)).IgnoreArguments().Return(Enumerable.Repeat(new Track(), expectedReleaseTrackCount).ToList());

			var productCollater = new ProductCollater(catalogue);
			var usingReleaseAndTrackId = productCollater.UsingReleaseId("GB", 12);

			Assert.That(usingReleaseAndTrackId.Release.Id, Is.EqualTo(TestRelease.FleetFoxes.Id));
			Assert.That(usingReleaseAndTrackId.Type, Is.EqualTo(PurchaseType.release));
			Assert.That(usingReleaseAndTrackId.Tracks.Count, Is.EqualTo(expectedReleaseTrackCount));
		}

		[Test]
		public void Collating_a_track_returns_release_and_only_that_track()
		{
			const int expectedReleaseTrackCount = 5;
			var fakeListOfTracks = Enumerable.Repeat(new Track(), expectedReleaseTrackCount).ToList();
			fakeListOfTracks.Add(TestTrack.SunItRises);

			var catalogue = MockRepository.GenerateStub<ICatalogue>();
			catalogue.Stub(x => x.GetARelease(null, 0)).IgnoreArguments().Return(TestRelease.FleetFoxes);
			catalogue.Stub(x => x.GetAReleaseTracks(null, 0)).IgnoreArguments().Return(fakeListOfTracks);
			catalogue.Stub(x => x.GetATrack(null, 0)).IgnoreArguments().Return(TestTrack.SunItRises);

			var productCollater = new ProductCollater(catalogue);
			var usingReleaseAndTrackId = productCollater.UsingTrackId("GB", TestTrack.SunItRises.Id);

			Assert.That(usingReleaseAndTrackId.Release.Id, Is.EqualTo(TestRelease.FleetFoxes.Id));
			Assert.That(usingReleaseAndTrackId.Type, Is.EqualTo(PurchaseType.track));
			Assert.That(usingReleaseAndTrackId.Tracks.Count, Is.EqualTo(1));
		}

		[Test]
		public void Collating_a_release_specific_track_returns_release_and_that_specific_track()
		{
			const int expectedReleaseTrackCount = 5;
			var fakeListOfTracks = Enumerable.Repeat(new Track(), expectedReleaseTrackCount).ToList();
			fakeListOfTracks.Add(TestTrack.SunItRises);

			var catalogue = MockRepository.GenerateStub<ICatalogue>();
			catalogue.Stub(x => x.GetARelease(null, 0)).IgnoreArguments().Return(TestRelease.FleetFoxes);
			catalogue.Stub(x => x.GetAReleaseTracks(null, 0)).IgnoreArguments().Return(fakeListOfTracks);
			catalogue.Stub(x => x.GetATrack(null, 0)).IgnoreArguments().Return(TestTrack.SunItRises);

			var productCollater = new ProductCollater(catalogue);
			var usingReleaseAndTrackId = productCollater.UsingReleaseAndTrackId("GB", 12, TestTrack.SunItRises.Id);

			Assert.That(usingReleaseAndTrackId.Release.Id, Is.EqualTo(TestRelease.FleetFoxes.Id));
			Assert.That(usingReleaseAndTrackId.Type, Is.EqualTo(PurchaseType.track));
			Assert.That(usingReleaseAndTrackId.Tracks.Count, Is.EqualTo(1));
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
			Assert.That(((HttpResult)o).Response, Is.TypeOf<BuyItNowResponse<ReleaseAndTracks>>());

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
			Assert.That(((HttpResult)o).Response, Is.TypeOf<BuyItNowResponse<ReleaseAndTracks>>());

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
	}
}