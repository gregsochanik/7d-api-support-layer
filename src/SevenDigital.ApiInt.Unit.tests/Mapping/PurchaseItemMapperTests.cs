using System.Collections.Generic;
using NUnit.Framework;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Unit.tests.Mapping
{
	[TestFixture]
	public class PurchaseItemMapperTests
	{
		[Test]
		public void Maps_release_from_locker()
		{
			var purchaseItemMapper = new PurchaseItemMapper();

			var expectedRelease = new Release
			{
				Id = 1,
				Title = "Hello"
			};
			
			var expectedTrack = new Track
			{
				Id = 1
			};

			var lockerRelease = new LockerRelease
			{
				Release = expectedRelease,
				LockerTracks = new List<LockerTrack> {new LockerTrack{Track = expectedTrack}}
			};

			var lockerReleases = new List<LockerRelease>{lockerRelease};
			var itemRequest = new ItemRequest
			{
				Type = PurchaseType.release,
				Id = 1,
			};
			
			var purchasedItem = purchaseItemMapper.Map(itemRequest, lockerReleases);

			Assert.That(purchasedItem.Id, Is.EqualTo(expectedRelease.Id));
			Assert.That(purchasedItem.Title, Is.EqualTo(expectedRelease.Title));
			Assert.That(purchasedItem.Tracks[0].Id, Is.EqualTo(lockerRelease.LockerTracks[0].Track.Id));
			Assert.That(purchasedItem.Tracks[0].Title, Is.EqualTo(lockerRelease.LockerTracks[0].Track.Title));
		}

		[Test]
		public void Maps_tracks_from_locker()
		{
			var purchaseItemMapper = new PurchaseItemMapper();

			var expectedTrack = new Track
			{
				Id = 1
			};

			var lockerRelease = new LockerRelease
			{
				Release = new Release(),
				LockerTracks = new List<LockerTrack> { new LockerTrack { Track = expectedTrack } }
			};

			var lockerReleases = new List<LockerRelease> { lockerRelease };
			var itemRequest = new ItemRequest
			{
				Type = PurchaseType.track,
				Id = 1,
			};

			var purchasedItem = purchaseItemMapper.Map(itemRequest, lockerReleases);

			Assert.That(purchasedItem.Id, Is.EqualTo(expectedTrack.Id));
			Assert.That(purchasedItem.Title, Is.EqualTo(expectedTrack.Title));
			Assert.That(purchasedItem.Tracks[0].Id, Is.EqualTo(lockerRelease.LockerTracks[0].Track.Id));
			Assert.That(purchasedItem.Tracks[0].Title, Is.EqualTo(lockerRelease.LockerTracks[0].Track.Title));
		}
	}
}
