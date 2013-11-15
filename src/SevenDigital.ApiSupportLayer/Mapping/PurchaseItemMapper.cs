using System.Collections.Generic;
using System.Linq;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Mapping
{
	public class PurchaseItemMapper : IPurchaseItemMapper
	{
		public PurchasedItem Map(ItemRequest request, IEnumerable<LockerRelease> lockerReleases)
		{
			return request.Type == PurchaseType.release 
				? ReleasePurchasedItem(request, lockerReleases) 
				: TrackPurchasedItem(request, lockerReleases);
		}

		private static PurchasedItem TrackPurchasedItem(IHasId request, IEnumerable<LockerRelease> lockerReleases)
		{
			var purchasedItem = new PurchasedItem();
			var lockerReleasesList = lockerReleases.ToList();
			var lockerTracks = lockerReleasesList.Select
			(
				lockerRelease => lockerRelease.LockerTracks
			);

			var lockerTracksAs7DTracks = lockerReleasesList.Select
			(
				lockerRelease => lockerRelease.LockerTracks.Select(TrackUtility.MergeInto7dTrack(lockerRelease))
			);

			var selectedTracks = lockerTracksAs7DTracks.SelectMany(x => x).Where(x => x.Id == request.Id).ToList();
			var selectedLockerTrack = lockerTracks.SelectMany(x => x).SingleOrDefault(x => x.Track.Id == request.Id);

			if (selectedTracks.Count > 0 && selectedLockerTrack != null)
			{
				purchasedItem.Id = selectedLockerTrack.Track.Id;
				purchasedItem.Title = selectedLockerTrack.Track.Title;
				purchasedItem.DownloadUrls = selectedLockerTrack.DownloadUrls;
				purchasedItem.Tracks = selectedTracks;
			}
			return purchasedItem;
		}

		private static PurchasedItem ReleasePurchasedItem(IHasId request, IEnumerable<LockerRelease> lockerReleases)
		{
			var purchasedItem = new PurchasedItem();

			var selectedLockerRelease = lockerReleases.SingleOrDefault(x => x.Release.Id == request.Id);
			
			if (selectedLockerRelease != null)
			{
				var selectedTracks = selectedLockerRelease.LockerTracks.Select(TrackUtility.MergeInto7dTrack(selectedLockerRelease));

				var purchasedItem1 = new PurchasedItem
				{
					Id = selectedLockerRelease.Release.Id,
					Title = selectedLockerRelease.Release.Title,
					DownloadUrls = selectedLockerRelease.LockerTracks[0].DownloadUrls,
					Tracks = selectedTracks.ToList()
				};
				purchasedItem = purchasedItem1;
			}

			return purchasedItem;
		}
	}
}