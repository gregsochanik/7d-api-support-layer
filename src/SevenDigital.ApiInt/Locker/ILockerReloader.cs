using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiInt.Locker
{
	public interface ILockerReloader
	{
		void FullLockerCacheRefreshAsync(OAuthAccessToken userAuthenticationDetails);
	}
}