using System;
using System.Collections.Generic;
using System.Linq;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Mapping
{
	public class PurchaseItemMapper : IPurchaseItemMapper
	{
		public PurchasedItem Map(ItemRequest request, IEnumerable<LockerRelease> lockerReleases)
		{
			var purchasedItem = new PurchasedItem();
			if (request.Type == PurchaseType.release)
			{
				var selectedLockerRelease = lockerReleases.SingleOrDefault(x => x.Release.Id == request.Id);
				if(selectedLockerRelease == null)
				{
					return new PurchasedItem();
				}

				purchasedItem.Id = selectedLockerRelease.Release.Id;
				purchasedItem.Title = selectedLockerRelease.Release.Title;
				purchasedItem.DownloadUrls = selectedLockerRelease.LockerTracks[0].DownloadUrls;
				purchasedItem.Tracks = selectedLockerRelease.LockerTracks.Select(TrackUtility.MergeInto7dTrack(selectedLockerRelease)).ToList();
			}
			else
			{
				var lockerReleasesList = lockerReleases.ToList();

				var lockerTracks = lockerReleasesList.Select
				(
					lockerRelease => lockerRelease.LockerTracks
				);

				var lockerTracksAs7DTracks = lockerReleasesList.Select
				(
					lockerRelease => lockerRelease.LockerTracks.Select(TrackUtility.MergeInto7dTrack(lockerRelease))
				);

				var selectedTrack = lockerTracksAs7DTracks.SelectMany(x => x).SingleOrDefault(x => x.Id == request.Id);
				var selectedLockerTrack = lockerTracks.SelectMany(x => x).SingleOrDefault(x => x.Track.Id == request.Id);

				if (selectedTrack == null || selectedLockerTrack == null)
				{
					return new PurchasedItem();
				}

				purchasedItem.Id = selectedLockerTrack.Track.Id;
				purchasedItem.Title = selectedLockerTrack.Track.Title;
				purchasedItem.DownloadUrls = selectedLockerTrack.DownloadUrls;
				purchasedItem.Tracks = new List<Track> { selectedTrack };
			}
			return purchasedItem;
		}
	}
}