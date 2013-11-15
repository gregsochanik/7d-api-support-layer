using System;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;

namespace SevenDigital.ApiSupportLayer.Mapping
{
	public class TrackUtility
	{
		public static Func<LockerTrack, Track> MergeInto7dTrack(LockerRelease lockerRelease)
		{
			return lockerTrack => new Track
			{
				Artist = lockerTrack.Track.Artist,
				Duration = lockerTrack.Track.Duration,
				ExplicitContent = lockerTrack.Track.ExplicitContent,
				Id = lockerTrack.Track.Id,
				Image = lockerTrack.Track.Image,
				Isrc = lockerTrack.Track.Isrc,
				Price = lockerTrack.Track.Price,
				Release = lockerRelease.Release,
				Title = lockerTrack.Track.Title,
				TrackNumber = lockerTrack.Track.TrackNumber,
				Type = lockerTrack.Track.Type,
				Version = lockerTrack.Track.Version
			};
		}
	}
}