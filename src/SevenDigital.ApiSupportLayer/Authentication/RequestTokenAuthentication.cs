using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Responses;
using SevenDigital.Api.Schema.Premium.OAuth;

namespace SevenDigital.ApiSupportLayer.Authentication
{
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