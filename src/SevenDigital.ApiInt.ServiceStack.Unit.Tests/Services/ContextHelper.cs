using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.Testing;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	public static class ContextHelper
	{
		public static MockRequestContext LoggedInContext()
		{
			var mockRequestContext = new MockRequestContext();
			var httpReq = mockRequestContext.Get<IHttpRequest>();
			var httpRes = mockRequestContext.Get<IHttpResponse>();
			var authUserSession = mockRequestContext.ReloadSession();
			authUserSession.Id = httpRes.CreateSessionId(httpReq);
			authUserSession.IsAuthenticated = true;
			authUserSession.ProviderOAuthAccess.Add(new OAuthTokens { AccessToken = "Token", AccessTokenSecret = "Secret" });

			httpReq.Items[ServiceExtensions.RequestItemsSessionKey] = authUserSession;
			return mockRequestContext;
		}
	}
}