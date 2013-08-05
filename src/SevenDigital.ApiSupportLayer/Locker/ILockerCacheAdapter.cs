using System;
using System.Collections.Generic;
using SevenDigital.Api.Schema.ArtistEndpoint;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.TrackEndpoint;

namespace SevenDigital.ApiSupportLayer.Locker
{
	public interface ILockerCacheAdapter
	{
		LockerStats GetLockerStats(OAuthAccessToken token);
		IEnumerable<LockerRelease> GetCachedLocker(OAuthAccessToken accessToken);
		IEnumerable<Artist> GetLockerArtists(OAuthAccessToken accessToken);
		IEnumerable<Track> GetLockerTracks(OAuthAccessToken accessToken);
		DateTime GetLatestPurchaseDateOfCachedLocker(OAuthAccessToken accessToken);
	}
}