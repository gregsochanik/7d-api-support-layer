using System;
using System.Collections.Generic;
using System.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using SevenDigital.ApiSupportLayer.Authentication;
using SevenDigital.ApiSupportLayer.User;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Authentication
{
	public class SevenDigitalCredentialsAuthProvider : CredentialsAuthProvider
	{
		private readonly IOAuthAuthentication _auth;
		private readonly IUserApi _userApi;
		private readonly ILog _logger;
		private static TimeSpan _sessionExpiry = new TimeSpan(0, 0, 15, 0);

		public SevenDigitalCredentialsAuthProvider(IOAuthAuthentication auth, IUserApi userApi)
		{
			_auth = auth;
			_userApi = userApi;
			_logger = LogManager.GetLogger(GetType());
		}

		public SevenDigitalCredentialsAuthProvider(IOAuthAuthentication auth, IUserApi userApi, TimeSpan sessionExpiry) 
			: this(auth, userApi)
		{
			_sessionExpiry = sessionExpiry;
		}

		public override bool TryAuthenticate(IServiceBase authService, string userName, string password)
		{
			try
			{
				var partnerId = authService.RequestContext.GetHeader("X-7d-partner-id") ?? string.Empty;

				if (!_userApi.CheckUserExists(userName))
				{
					_userApi.Create(userName, password, partnerId);
				}
				var oAuthAccessToken = _auth.ForUser(HttpUtility.UrlEncode(userName), HttpUtility.UrlEncode(password));

				var session = authService.GetSession();
				session.IsAuthenticated = true;
				session.ProviderOAuthAccess = new List<IOAuthTokens>
				{
					new OAuthTokens
					{
						AccessToken = oAuthAccessToken.Token,
						AccessTokenSecret = oAuthAccessToken.Secret
					}
				};
				
				return true;
			}
			catch (LoginInvalidException ex)
			{
				_logger.Info("Login failed", ex);
				return false;
			}
		}

		public override void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, Dictionary<string, string> authInfo)
		{
			session.IsAuthenticated = true;

			SessionExpiry = _sessionExpiry;
			
			base.OnAuthenticated(authService, session, tokens, authInfo);
		}

		public override void OnFailedAuthentication(IAuthSession session, global::ServiceStack.ServiceHost.IHttpRequest httpReq, global::ServiceStack.ServiceHost.IHttpResponse httpRes)
		{
			base.OnFailedAuthentication(session, httpReq, httpRes);
		}
	}
}