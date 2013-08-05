using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiInt.Authentication
{
	public interface IOAuthAuthentication
	{
		OAuthAccessToken ForUser(string username, string password);
	}
}