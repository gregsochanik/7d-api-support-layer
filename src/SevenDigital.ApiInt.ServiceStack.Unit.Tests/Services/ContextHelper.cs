using System;
using System.Net;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.Testing;
using SevenDigital.ApiInt.ServiceStack.Services;
using SevenDigital.ApiInt.TestData;

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
			authUserSession.ProviderOAuthAccess.Add(new OAuthTokens { AccessToken = FakeUserData.FakeAccessToken.Token, AccessTokenSecret = FakeUserData.FakeAccessToken.Secret });

			httpReq.Items[ServiceExtensions.RequestItemsSessionKey] = authUserSession;
			return mockRequestContext;
		}

		public static MockRequestContext LoggedInContextWithFakeBasketCookie(Guid fakeBasketId)
		{
			var mockRequestContext = LoggedInContext();
			mockRequestContext.Cookies.Add(StateHelper.BASKET_COOKIE_NAME, new Cookie(StateHelper.BASKET_COOKIE_NAME, fakeBasketId.ToString()));
			return mockRequestContext;
		}
	}
}