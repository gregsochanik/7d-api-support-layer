using SevenDigital.Api.Schema.Pricing;

namespace SevenDigital.ApiSupportLayer.Model
{
	public static class ReleaseAndTracksExtension
	{
		public static bool IsABundleTrack(this ReleaseAndTracks collatedReleaseAndTracks)
		{
			if (collatedReleaseAndTracks == null || collatedReleaseAndTracks.Tracks == null)
				return false;

			if (collatedReleaseAndTracks.Tracks.Count < 1)
				return false;

			if (collatedReleaseAndTracks.Tracks[0].Price == null)
				return false;

			return collatedReleaseAndTracks.Type == PurchaseType.track 
			       && collatedReleaseAndTracks.Tracks[0].Price.Status == PriceStatus.UnAvailable;
		}
	}
}