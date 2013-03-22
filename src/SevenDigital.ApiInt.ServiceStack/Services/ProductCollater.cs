using System.Linq;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class ProductCollater : IProductCollater
	{
		private readonly ICatalogue _catalogue;

		public ProductCollater(ICatalogue catalogue)
		{
			_catalogue = catalogue;
		}

		public ReleaseAndTracks UsingReleaseId(string countryCode, int releaseId)
		{
			return new ReleaseAndTracks
			{
				Type = PurchaseType.release,
				Release = _catalogue.GetARelease(countryCode, releaseId),
				Tracks = _catalogue.GetAReleaseTracks(countryCode, releaseId)
			};
		}

		public ReleaseAndTracks UsingTrackId(string countryCode, int trackId)
		{
			var aTrack = _catalogue.GetATrack(countryCode, trackId);
			var releaseTracks = _catalogue.GetAReleaseTracks(countryCode, aTrack.Release.Id);
			var aRelease = _catalogue.GetARelease(countryCode, aTrack.Release.Id);

			return new ReleaseAndTracks
			{
				Type = PurchaseType.track,
				Release = aRelease,
				Tracks = releaseTracks.Where(x => x.Id == trackId).ToList()
			};
		}

		public ReleaseAndTracks UsingReleaseAndTrackId(string countryCode, int releaseId, int trackId)
		{
			var releaseTracks = _catalogue.GetAReleaseTracks(countryCode, releaseId);
			var aRelease = _catalogue.GetARelease(countryCode, releaseId);

			return new ReleaseAndTracks
			{
				Type = PurchaseType.track,
				Release = aRelease,
				Tracks = releaseTracks.Where(x => x.Id == trackId).ToList()
			};
		}
	}
}