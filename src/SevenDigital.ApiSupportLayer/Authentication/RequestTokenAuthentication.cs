using System.Collections.Generic;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;
using SevenDigital.Api.Wrapper.Http;

namespace SevenDigital.ApiSupportLayer.Authentication
{
	public class RequestTokenAuthentication : IRequestTokenAuthentication
	{
		private readonly IApiUri _apiUri;
		private readonly IOAuthCredentials _oAuthCredentials;
		private readonly IUrlSigner _urlSigner;
		private readonly IHttpClient _httpClient;

		private const string REQUEST_TOKEN_URL = "/oauth/requestToken/authorise";

		public RequestTokenAuthentication(IApiUri apiUri, IOAuthCredentials oAuthCredentials, IUrlSigner urlSigner, IHttpClient httpClient)
		{
			_apiUri = apiUri;
			_oAuthCredentials = oAuthCredentials;
			_urlSigner = urlSigner;
			_httpClient = httpClient;
		}

		public Response Authorise(OAuthRequestToken oAuthRequestToken, string username, string password)
		{
			string oauthRequesttokenAuthorise = _apiUri.SecureUri + REQUEST_TOKEN_URL;

			var postParameters = new Dictionary<string, string>
			{
				{"token", oAuthRequestToken.Token}, 
				{"username", username}, 
				{"password", password},
			};

			var signPostRequest = _urlSigner.SignPostRequest(oauthRequesttokenAuthorise, "", "", _oAuthCredentials, postParameters);

			var postRequest = new PostRequest(oauthRequesttokenAuthorise, new Dictionary<string, string>(), signPostRequest);
			return _httpClient.Post(postRequest);
		}
	}
}