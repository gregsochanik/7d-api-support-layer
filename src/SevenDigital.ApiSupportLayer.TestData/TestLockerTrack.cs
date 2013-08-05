using System;
using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;

namespace SevenDigital.ApiSupportLayer.TestData
{
	public static class TestLockerTrack
	{
		public static LockerTrack Get()
		{
			return new LockerTrack
			{
			    Track = TestTrack.SunItRises,
			    DownloadUrls = new List<DownloadUrl> { new DownloadUrl { Url = "meh", Format = TestFormat.Mp3} },
			    PurchaseDate = DateTime.Now.AddDays(-1),
			    RemainingDownloads = 10
			};
		}

		public static LockerTrack Get(int trackId, string trackTitle)
		{
			var lockerTrack = new LockerTrack
			                  {
			                  	Track = TestTrack.SunItRises,
			                  	DownloadUrls = new List<DownloadUrl>
			                  	               {
			                  	               	new DownloadUrl
			                  	               	{
			                  	               		Url = "meh",
			                  	               		Format = TestFormat.Mp3
			                  	               	}
			                  	               },
			                  	PurchaseDate = DateTime.Now.AddDays(-1),
			                  	RemainingDownloads = 10
			                  };

			lockerTrack.Track.Id = trackId;
			lockerTrack.Track.Title = trackTitle;
			return lockerTrack;
		}

		public static LockerTrack TrackByKeane()
		{
			var lockerTrack = new LockerTrack
			{
				Track = TestTrack.EverybodysChanging,
				DownloadUrls = new List<DownloadUrl>
			                  	               {
			                  	               	new DownloadUrl
			                  	               	{
			                  	               		Url = "meh",
			                  	               		Format = TestFormat.Mp3
			                  	               	}
			                  	               },
				PurchaseDate = DateTime.Now.AddDays(-1),
				RemainingDownloads = 10
			};
			return lockerTrack;
		}

		public static LockerTrack TrackByFleetFoxes()
		{
			var lockerTrack = new LockerTrack
			{
				Track = TestTrack.SunItRises,
				DownloadUrls = new List<DownloadUrl>
			                  	               {
			                  	               	new DownloadUrl
			                  	               	{
			                  	               		Url = "meh",
			                  	               		Format = TestFormat.Mp3
			                  	               	}
			                  	               },
				PurchaseDate = DateTime.Now.AddDays(-1),
				RemainingDownloads = 10
			};
			return lockerTrack;
		}

		public static LockerTrack GetNonAudioTrack()
		{
			return new LockerTrack
			{
				Track = TestTrack.NonAudioTrack,
				DownloadUrls = new List<DownloadUrl> { new DownloadUrl { Url = "meh", Format = TestFormat.Pdf } },
				PurchaseDate = DateTime.Now.AddDays(-1),
				RemainingDownloads = 10
			};
		}
	}
}