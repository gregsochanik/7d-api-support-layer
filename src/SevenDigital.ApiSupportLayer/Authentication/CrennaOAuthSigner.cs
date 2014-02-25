using System.Collections.Generic;
using OAuth;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Requests;

namespace SevenDigital.ApiSupportLayer.Authentication
{
	public class CrennaOAuthSigner : IOAuthSigner
	{
		private readonly IOAuthCredentials _configAuthCredentials;

		public CrennaOAuthSigner(IOAuthCredentials configAuthCredentials)
		{
			_configAuthCredentials = configAuthCredentials;
		}

		public string SignGetRequest(string unsignedUrl, OAuthAccessToken oAuthAccessToken, Dictionary<string, string> parameters)
		{
			var oauthRequest = new OAuthRequest
			{
				Type = OAuthRequestType.ProtectedResource,
				RequestUrl = unsignedUrl,
				Method = "GET",
				ConsumerKey = _configAuthCredentials.ConsumerKey,
				ConsumerSecret = _configAuthCredentials.ConsumerSecret,
				Token = oAuthAccessToken.Token,
				TokenSecret = oAuthAccessToken.Secret
			};

			var authorizationQuery = oauthRequest.GetAuthorizationQuery(parameters);

			return string.Format("{0}?{1}{2}", oauthRequest.RequestUrl, authorizationQuery, parameters.ToQueryString());
		}
	}
}