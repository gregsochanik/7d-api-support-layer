using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiSupportLayer.Locker
{
	public interface ILockerReloader
	{
		void FullLockerCacheRefreshAsync(OAuthAccessToken userAuthenticationDetails);
	}
}