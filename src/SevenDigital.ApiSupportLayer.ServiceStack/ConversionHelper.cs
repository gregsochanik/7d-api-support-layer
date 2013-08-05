using ServiceStack.ServiceInterface.Auth;
using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiSupportLayer.ServiceStack
{
	public static class ConversionHelper
	{
		public static OAuthAccessToken Extract7dAccessTokenFromSession(IAuthSession authSession)
		{
			var accessToken = authSession.ProviderOAuthAccess[0].AccessToken;
			var accessTokenSecret = authSession.ProviderOAuthAccess[0].AccessTokenSecret;
			return new OAuthAccessToken { Token = accessToken, Secret = accessTokenSecret };
		}
	}
}