using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiSupportLayer.Authentication
{
	public interface IOAuthAuthentication
	{
		OAuthAccessToken ForUser(string username, string password);
	}
}