using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiInt.Locker
{
	public interface ITimedCacheReloading
	{
		TResult TimedSynchronousCacheGet<TResult>(OAuthAccessToken accessToken, string key, int timeout)
			where TResult : class;
	}
}