using System;
using SevenDigital.Api.Schema.Attributes;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Responses;

namespace SevenDigital.ApiSupportLayer.Authentication
{
	[ApiEndpoint("oauth/requestToken/authorise")]
	[Serializable]
	[HttpPost]
	[OAuthSigned]
	[RequireSecure]
	public class RequestTokenAuthorise
	{ }

	public class RequestTokenAuthentication : IRequestTokenAuthentication
	{
		private readonly IFluentApi<RequestTokenAuthorise> _requestTokenApi;
		
		public RequestTokenAuthentication(IFluentApi<RequestTokenAuthorise> requestTokenApi)
		{
			_requestTokenApi = requestTokenApi;
		}

		public Response Authorise(OAuthRequestToken oAuthRequestToken, string username, string password)
		{
			return _requestTokenApi
				.WithParameter("token", oAuthRequestToken.Token)
				.WithParameter("username", username)
				.WithParameter("password", password)
				.Response();
		}
	}
}