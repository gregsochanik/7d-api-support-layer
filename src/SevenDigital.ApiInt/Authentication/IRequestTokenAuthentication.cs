using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper.Http;

namespace SevenDigital.ApiInt.Authentication
{
	public interface IRequestTokenAuthentication
	{
		Response Authorise(OAuthRequestToken oAuthRequestToken, string username, string password);
	}
}