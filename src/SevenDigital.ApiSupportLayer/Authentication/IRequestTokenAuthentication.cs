using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper.Responses;

namespace SevenDigital.ApiSupportLayer.Authentication
{
	public interface IRequestTokenAuthentication
	{
		Response Authorise(OAuthRequestToken oAuthRequestToken, string username, string password);
	}
}