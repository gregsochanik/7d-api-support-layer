using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiSupportLayer.Locker
{
	public interface ITimedCacheReloading
	{
		TResult TimedSynchronousCacheGet<TResult>(OAuthAccessToken accessToken, string key, int timeout)
			where TResult : class;
	}
}