using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiSupportLayer.ServiceStack
{
	public static class ServiceSessionExtensions
	{
		public static OAuthAccessToken TryGetOAuthAccessToken(this Service service)
		{
			var authSession = service.GetSession();
			if (authSession == null || authSession.IsAuthenticated == false)
			{
				throw new HttpError(HttpStatusCode.Unauthorized, "User not logged in");
			}
			return Extract7dAccessTokenFromSession(authSession);
		}

		public static OAuthAccessToken Extract7dAccessTokenFromSession(IAuthSession authSession)
		{
			var accessToken = authSession.ProviderOAuthAccess[0].AccessToken;
			var accessTokenSecret = authSession.ProviderOAuthAccess[0].AccessTokenSecret;
			return new OAuthAccessToken { Token = accessToken, Secret = accessTokenSecret };
		}
	}
}