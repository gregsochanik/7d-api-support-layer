using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiInt.ServiceStack.Services
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
			return ConversionHelper.Extract7dAccessTokenFromSession(authSession);
		}
	}
}