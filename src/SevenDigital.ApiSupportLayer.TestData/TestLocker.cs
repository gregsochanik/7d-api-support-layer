using System;
using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;

namespace SevenDigital.ApiSupportLayer.TestData
{
	public static class TestLocker
	{
		public static LockerResponse Get(int release, int tracks)
		{
			var lockerReleases = new List<LockerRelease>();

			for (var i = 0; i < release; i++)
			{
				var lockerTracks = new List<LockerTrack>();

				for (var j = 0; j < tracks; j++)
				{
					var lockerTrack = TestLockerTrack.Get(i + j, string.Format("Track_{0}_{1}", i, j));

					lockerTracks.Add(lockerTrack);
				}
				
				lockerReleases.Add(new LockerRelease
				{
				    Release = TestRelease.Get(1000+i, "Album number "+i),
					LockerTracks = lockerTracks
				});
			}
			return new LockerResponse
			{
			    LockerReleases = lockerReleases,
			    Page = 1,
			    PageSize = 10,
				TotalItems = release
			};
		}
		
		public static LockerResponse GetLockerWithReleaseBy2DifferentArtists(int numTracksOnEach)
		{
			var lockerReleases = new List<LockerRelease>();

			var lockerTracksKeane = new List<LockerTrack>();
			var lockerTracksFleetFoxes = new List<LockerTrack>();

			for (var j = 0; j < numTracksOnEach; j++)
			{
				lockerTracksKeane.Add(TestLockerTrack.TrackByKeane());
			}

			for (var j = 0; j < numTracksOnEach; j++)
			{
				lockerTracksFleetFoxes.Add(TestLockerTrack.TrackByFleetFoxes());
			}

			lockerReleases.Add(new LockerRelease
			{
				Release = TestRelease.HopesAndFears,
				LockerTracks = lockerTracksKeane
			});

			lockerReleases.Add(new LockerRelease
			{
				Release = TestRelease.FleetFoxes,
				LockerTracks = lockerTracksFleetFoxes
			});

			return new LockerResponse
			{
				LockerReleases = lockerReleases,
				Page = 1,
				PageSize = 10,
				TotalItems = 2
			};
		}

		public static LockerResponse GetLockerWithReleasesWithDifferentPurchaseDates(IDictionary<DateTime, string> purchaseDates)
		{
			var lockerReleases = new List<LockerRelease>();

			var counter = 1;
			foreach (var purchaseDate in purchaseDates)
			{

				var lockerRelease = new LockerRelease
				                    {
				                    	Release = TestRelease.FleetFoxes,
				                    };
				var lockerTracks = new List<LockerTrack>
				                   {
										TestLockerTrack.Get()
				                   };
				lockerTracks[0].PurchaseDate = purchaseDate.Key;
				lockerRelease.LockerTracks = lockerTracks;
				lockerRelease.Release.Artist.Name = purchaseDate.Value;
				lockerRelease.Release.Artist.Id = counter;
				lockerReleases.Add(lockerRelease);
				counter++;
			}

			return new LockerResponse
			{
				LockerReleases = lockerReleases,
				Page = 1,
				PageSize = 10,
				TotalItems = purchaseDates.Count
			};
		}

		public static LockerResponse GetLockerWithPreReleases(int numberOfNonPreReleases, int numberOfPreReleases, int numTracksOnEach)
		{
			var lockerTrack = TestLockerTrack.Get();

			var lockerReleases = new List<LockerRelease>();

			var lockerTracks = new List<LockerTrack>();

			for (var j = 0; j < numTracksOnEach; j++)
			{
				lockerTracks.Add(lockerTrack);
			}

			for (var i = 0; i < numberOfNonPreReleases; i++)
			{
				lockerReleases.Add(new LockerRelease
				{
					Release = TestRelease.FleetFoxes,
					LockerTracks = lockerTracks
				});
			}

			for (int i = 0; i < numberOfPreReleases; i++)
			{
				lockerReleases.Add(new LockerRelease
				{
					Release = TestRelease.FutureRelease,
					LockerTracks = lockerTracks
				});
			}

			return new LockerResponse
			{
				LockerReleases = lockerReleases,
				Page = 1,
				PageSize = 10,
				TotalItems = lockerReleases.Count
			};
		}

		public static LockerResponse GetLockerWithReleasesWithAudioTracksAndNonAudioTracks(int releasesWithAUdioTracks, int releasesWithNoAudioTracks)
		{
			var audioLockerTrack = TestLockerTrack.Get();
			var nonAudioLockerTrack = TestLockerTrack.GetNonAudioTrack();

			var lockerReleases = new List<LockerRelease>();

			var audiolockerTracks = new List<LockerTrack>();

			for (var j = 0; j < 2; j++)
			{
				audiolockerTracks.Add(audioLockerTrack);
			}

			var nonAudiolockerTracks = new List<LockerTrack>();

			for (var j = 0; j < 2; j++)
			{
				nonAudiolockerTracks.Add(nonAudioLockerTrack);
			}

			for (var i = 0; i < releasesWithAUdioTracks; i++)
			{
				lockerReleases.Add(new LockerRelease
				{
					Release = TestRelease.FleetFoxes,
					LockerTracks = audiolockerTracks
				});
			}

			for (int i = 0; i < releasesWithNoAudioTracks; i++)
			{
				lockerReleases.Add(new LockerRelease
				{
					Release = TestRelease.FleetFoxes,
					LockerTracks = nonAudiolockerTracks
				});
			}

			return new LockerResponse
			{
				LockerReleases = lockerReleases,
				Page = 1,
				PageSize = 10,
				TotalItems = lockerReleases.Count
			};
		}
	}
}