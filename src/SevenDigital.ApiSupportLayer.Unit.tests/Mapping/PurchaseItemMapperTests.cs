using System.Collections.Generic;
using NUnit.Framework;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.Media;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.ApiSupportLayer.Mapping;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Unit.Tests.Mapping
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
				Title = "Hello",
			};
			
			var expectedTrack = new Track
			{
				Id = 1
			};

			var lockerRelease = new LockerRelease
			{
				Release = expectedRelease,
				LockerTracks = new List<LockerTrack> {new LockerTrack{Track = expectedTrack, DownloadUrls = new List<DownloadUrl>{new DownloadUrl(){Format = new Format(){BitRate = "320"}}}}}
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
			Assert.That(purchasedItem.DownloadUrls, Is.EqualTo(lockerRelease.LockerTracks[0].DownloadUrls));
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
				Release = new Release()
				{
					Formats = new FormatList { Formats = new List<Format> { new Format { BitRate = "320" } } }
				},
				LockerTracks = new List<LockerTrack> { new LockerTrack { Track = expectedTrack, DownloadUrls = new List<DownloadUrl>{new DownloadUrl(){Format = new Format{BitRate = "320"}}}}}
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
			Assert.That(purchasedItem.DownloadUrls, Is.EqualTo(lockerRelease.LockerTracks[0].DownloadUrls));

			Assert.That(purchasedItem.Tracks[0].Id, Is.EqualTo(lockerRelease.LockerTracks[0].Track.Id));
			Assert.That(purchasedItem.Tracks[0].Title, Is.EqualTo(lockerRelease.LockerTracks[0].Track.Title));
		}

		[Test]
		public void Returns_empty_purchasedItem_if_track_not_found()
		{
			var purchaseItemMapper = new PurchaseItemMapper();
			
			var lockerReleases = LockerReleases();
			var itemRequest = new ItemRequest
			{
				Type = PurchaseType.track,
				Id = -1,
			};

			var purchasedItem = purchaseItemMapper.Map(itemRequest, lockerReleases);

			Assert.That(purchasedItem.Id, Is.EqualTo(0));
			Assert.That(purchasedItem.Title, Is.EqualTo(""));
		}

		[Test]
		public void Returns_empty_purchasedItem_if_release_not_found()
		{
			var purchaseItemMapper = new PurchaseItemMapper();

			var lockerReleases = LockerReleases();
			var itemRequest = new ItemRequest
			{
				Type = PurchaseType.release,
				Id = -1,
			};

			var purchasedItem = purchaseItemMapper.Map(itemRequest, lockerReleases);

			Assert.That(purchasedItem.Id, Is.EqualTo(0));
			Assert.That(purchasedItem.Title, Is.EqualTo(""));
		}

		private static IEnumerable<LockerRelease> LockerReleases()
		{
			var lockerRelease = new LockerRelease
			{
				Release = new Release
				{
					Formats = new FormatList
					{
						Formats = new List<Format>
						{
							new Format
							{
								BitRate = "320"
							}
						}
					}
				},
				LockerTracks = new List<LockerTrack>
				{
					new LockerTrack
					{
						Track = new Track(),
						DownloadUrls = new List<DownloadUrl>
						{
							new DownloadUrl
							{
								Format = new Format
								{
									BitRate = "320"
								}
							}
						}
					}
				}
			};

			var lockerReleases = new List<LockerRelease>
			{
				lockerRelease
			};
			return lockerReleases;
		}
	}
}
