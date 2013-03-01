using System.Diagnostics;
using System.Timers;
using ServiceStack.CacheAccess;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiInt.Locker;

namespace SevenDigital.ApiInt.ServiceStack.Cache
{
	public class TimedCacheReloading : ITimedCacheReloading
	{
		private readonly ICacheClient _cacheClient;
		private readonly ILockerReloader _lockerReloader;

		public TimedCacheReloading(ICacheClient cacheClient, ILockerReloader lockerReloader)
		{
			_cacheClient = cacheClient;
			_lockerReloader = lockerReloader;
		}

		public TResult TimedSynchronousCacheGet<TResult>(OAuthAccessToken accessToken, string key, int timeout)
			where TResult : class
		{
			var result = _cacheClient.Get<TResult>(key);

			if (result == null)
			{
				_lockerReloader.FullLockerCacheRefreshAsync(accessToken);
			}

			return result ?? RepeatedlyAttemptToGetFromCache<TResult>(key, timeout);

		}

		private TResult RepeatedlyAttemptToGetFromCache<TResult>(string key, int timeout) where TResult : class
		{
			var sw = new Stopwatch();
			sw.Start();

			var result = _cacheClient.Get<TResult>(key);

			var timer = new Timer(500);
			timer.Elapsed += (sender, args) => { result = _cacheClient.Get<TResult>(key); };
			timer.Start();

			while (result == null && sw.Elapsed.Seconds < timeout)
			{ }

			timer.Stop();
			sw.Reset();
			return result;
		}
	}
}