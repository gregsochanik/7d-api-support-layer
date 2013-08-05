using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.CacheAccess;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.Locker;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Cache
{
	public class LockerReloader : ILockerReloader
	{
		private readonly ILockerRetrieval _lockerRetrieval;
		private readonly ICacheClient _cacheClient;
		private readonly ICacheLock _cacheLock;
		private readonly IUserTokenCache _userTokenCache;

		public LockerReloader(ILockerRetrieval lockerRetrieval, ICacheClient cacheClient, ICacheLock cacheLock, IUserTokenCache userTokenCache)
		{
			_lockerRetrieval = lockerRetrieval;
			_cacheClient = cacheClient;
			_cacheLock = cacheLock;
			_userTokenCache = userTokenCache;
		}

		public void FullLockerCacheRefreshAsync(OAuthAccessToken userAuthenticationDetails)
		{
			var username = _userTokenCache.GetUsernameForToken(userAuthenticationDetails);
			Task.Factory.StartNew(() => ReloadEntireLockerCache(username, userAuthenticationDetails));
		}

		private void ReloadEntireLockerCache(string username, OAuthAccessToken userAuthenticationDetails)
		{
			_cacheLock.PerformLockedTask(username, () => DownloadLocker(username, userAuthenticationDetails));
		}

		private void DownloadLocker(string username, OAuthAccessToken userAuthenticationDetails)
		{
			var lockerStats = _lockerRetrieval.GetLockerStats(userAuthenticationDetails);
			var lockerReleases = _lockerRetrieval.GetLockerReleases(userAuthenticationDetails, lockerStats);
			SetLockerCacheItems(username, lockerReleases, lockerStats);
		}

		private void SetLockerCacheItems(string username, IEnumerable<LockerRelease> lockerReleases, LockerStats totalItems)
		{
			var cacheTtl = TimeSpan.FromDays(7);
			var lockerCacheKey = CacheKeys.LockerCacheKey(username);
			var totalItemsCacheKey = CacheKeys.LockerTotalItemsCacheKey(username);
			_cacheClient.Set(totalItemsCacheKey, totalItems, cacheTtl);
			_cacheClient.Set(lockerCacheKey, lockerReleases, cacheTtl);
		}
	}
}