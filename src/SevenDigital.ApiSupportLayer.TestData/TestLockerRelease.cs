using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;

namespace SevenDigital.ApiSupportLayer.TestData
{
	public static class TestLockerRelease
	{
		public static LockerRelease GetLockerWithReleaseContainingNonAudio(int numAudio, int numNonAudio)
		{
			var lockerTracks = new List<LockerTrack> ();
			for (int i = 0; i < numAudio; i++)
			{
				lockerTracks.Add(TestLockerTrack.Get());
			}
			for (int i = 0; i < numNonAudio; i++)
			{
				lockerTracks.Add(TestLockerTrack.GetNonAudioTrack()); 
			}

			return new LockerRelease
			       	{
			       		Release = TestRelease.FleetFoxes,
			       		LockerTracks = lockerTracks
			       	};
		}

		public static LockerRelease GetLockerWithReleaseContainingPreReleases()
		{
			var lockerTracks = new List<LockerTrack> { TestLockerTrack.Get(), TestLockerTrack.Get() };
			return new LockerRelease
			{
				Release = TestRelease.FleetFoxes,
				LockerTracks = lockerTracks
			};
		}
	}
}