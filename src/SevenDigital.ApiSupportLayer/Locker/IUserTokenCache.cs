using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiSupportLayer.Locker
{
	public interface IUserTokenCache
	{
		string GetUsernameForToken(OAuthAccessToken token);
		void SetUsernameForToken(OAuthAccessToken token, string username);
	}
}