using System;
using System.Collections.Generic;
using System.Linq;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Mapping
{
	public class PurchaseItemMapper : IPurchaseItemMapper
	{
		public PurchasedItem Map(ItemRequest request, IEnumerable<LockerRelease> lockerReleases)
		{
			var purchasedItem = new PurchasedItem();
			if (request.Type == PurchaseType.release)
			{
				var selectedLockerRelease = lockerReleases.FirstOrDefault(x => x.Release.Id == request.Id);
				if(selectedLockerRelease == null)
					throw new Exception(string.Format("release {0} not found in lockerReleases", request.Id));

				purchasedItem.Id = selectedLockerRelease.Release.Id;
				purchasedItem.Title = selectedLockerRelease.Release.Title;
				purchasedItem.AvailableFormats = selectedLockerRelease.Release.Formats.Formats;
				purchasedItem.Tracks = selectedLockerRelease.LockerTracks.Select(TrackUtility.MergeInto7dTrack(selectedLockerRelease)).ToList();
			}
			else
			{
				var lockerTracksAs7DTracks = lockerReleases.Select
				(
					lockerRelease => lockerRelease.LockerTracks.Select(TrackUtility.MergeInto7dTrack(lockerRelease))
				);
				var selectedTrack = lockerTracksAs7DTracks.SelectMany(x => x).FirstOrDefault(x => x.Id == request.Id);
				if (selectedTrack == null)
					throw new Exception(string.Format("track {0} not found in lockerTracks", request.Id));

				purchasedItem.Id = selectedTrack.Id;
				purchasedItem.Title = selectedTrack.Title;
				purchasedItem.AvailableFormats = selectedTrack.Release.Formats.Formats;
				purchasedItem.Tracks = new List<Track> { selectedTrack };
			}
			return purchasedItem;
		}
	}
}