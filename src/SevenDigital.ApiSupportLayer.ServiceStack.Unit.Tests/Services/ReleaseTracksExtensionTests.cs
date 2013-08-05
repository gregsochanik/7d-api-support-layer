using System.Collections.Generic;
using NUnit.Framework;
using SevenDigital.Api.Schema.Pricing;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class ReleaseTracksExtensionTests
	{
		[Test]
		public void Returns_false_on_blank_object()
		{
			var releaseAndTracks = new ReleaseAndTracks();
			Assert.That(releaseAndTracks.IsABundleTrack(), Is.False);
		}

		[Test]
		public void Returns_false_on_Type_track_null_price_object()
		{
			var releaseAndTracks = new ReleaseAndTracks(){Type=PurchaseType.track};
			Assert.That(releaseAndTracks.IsABundleTrack(), Is.False);
		}

		[Test]
		public void Returns_true_on_bundleTrack_object()
		{
			var releaseAndTracks = new ReleaseAndTracks{Type = PurchaseType.track, Tracks = new List<Track> { new Track { Price=new Price{Value = "", FormattedPrice = "N/A"}}}};
			Assert.That(releaseAndTracks.IsABundleTrack());
		}
	}
}