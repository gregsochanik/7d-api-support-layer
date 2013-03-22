using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Catalogue;
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
}