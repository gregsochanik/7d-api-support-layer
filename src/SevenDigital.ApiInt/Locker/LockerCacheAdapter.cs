using System;
using System.Collections.Generic;
using System.Linq;
using SevenDigital.Api.Schema.ArtistEndpoint;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Mapping;

namespace SevenDigital.ApiInt.Locker
{
	public class LockerCacheAdapter : ILockerCacheAdapter
	{
		private readonly ITimedCacheReloading _timedCacheReloading;
		private readonly IUserTokenCache _userTokenCache;

		public LockerCacheAdapter(ITimedCacheReloading timedCacheReloading, IUserTokenCache userTokenCache)
		{
			_timedCacheReloading = timedCacheReloading;
			_userTokenCache = userTokenCache;
		}

		public LockerStats GetLockerStats(OAuthAccessToken accessToken)
		{
			var username = _userTokenCache.GetUsernameForToken(accessToken);
			var key = CacheKeys.LockerTotalItemsCacheKey(username);
			return _timedCacheReloading.TimedSynchronousCacheGet<LockerStats>(accessToken, key, 30);
		}

		public IEnumerable<LockerRelease> GetCachedLocker(OAuthAccessToken accessToken)
		{
			var username = _userTokenCache.GetUsernameForToken(accessToken);
			var key = CacheKeys.LockerCacheKey(username);
			return _timedCacheReloading.TimedSynchronousCacheGet<IEnumerable<LockerRelease>>(accessToken, key, 30);
		}

		public IEnumerable<Artist> GetLockerArtists(OAuthAccessToken accessToken)
		{
			var lockerTracks = GetLockerTracks(accessToken).ToList();

			return lockerTracks.GroupBy(x => x.Artist.Id).Select(grouping => lockerTracks.FirstOrDefault(x => x.Artist.Id == grouping.Key).Artist).OrderBy(x => x.Name);
		}

		public IEnumerable<Track> GetLockerTracks(OAuthAccessToken accessToken)
		{
			var lockerReleases = GetCachedLocker(accessToken);

			var lockerTracksAs7DTracks = lockerReleases.Select
				(
					lockerRelease => lockerRelease.LockerTracks.Select(TrackUtility.MergeInto7dTrack(lockerRelease))
				);
			return lockerTracksAs7DTracks.SelectMany(x => x);
		}

		public DateTime GetLatestPurchaseDateOfCachedLocker(OAuthAccessToken accessToken)
		{
			var lockerReleases = GetCachedLocker(accessToken).ToList();

			if (!lockerReleases.Any())
				return DateTime.MinValue;

			return lockerReleases.First()
			                     .LockerTracks.OrderByDescending(x => x.PurchaseDate).First()
			                     .PurchaseDate;
		}
	}
}