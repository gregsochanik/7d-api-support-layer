using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiInt.Locker
{
	public interface IUserTokenCache
	{
		string GetUsernameForToken(OAuthAccessToken token);
		void SetUsernameForToken(OAuthAccessToken token, string username);
	}
}