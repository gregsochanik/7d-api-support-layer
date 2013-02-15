using System;
using System.Collections.Generic;
using System.Web;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using SevenDigital.ApiInt.Authentication;
using SevenDigital.ApiInt.User;

namespace SevenDigital.ApiInt.ServiceStack.Authentication
{
	public class SevenDigitalCredentialsAuthProvider : CredentialsAuthProvider
	{
		private readonly IOAuthAuthentication _auth;
		private readonly IUserApi _userApi;
		private readonly ILog _logger;

		public SevenDigitalCredentialsAuthProvider(IOAuthAuthentication auth, IUserApi userApi)
		{
			_auth = auth;
			_userApi = userApi;
			_logger = LogManager.GetLogger(GetType());
		}

		public override bool TryAuthenticate(IServiceBase authService, string userName, string password)
		{
			try
			{
				if (!_userApi.CheckUserExists(userName))
				{
					_userApi.Create(userName, password);
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
				_logger.Info("Login failed");

				throw HttpError.Unauthorized(ex.Message);
			}
		}

		public override void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, Dictionary<string, string> authInfo)
		{
			session.IsAuthenticated = true;

			SessionExpiry = new TimeSpan(1, 0, 0, 0);
			
			base.OnAuthenticated(authService, session, tokens, authInfo);
		}
	}
}