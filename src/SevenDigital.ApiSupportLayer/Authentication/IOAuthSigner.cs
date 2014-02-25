using System.Collections.Generic;
using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiSupportLayer.Authentication
{
	public interface IOAuthSigner
	{
		string SignGetRequest(string unsignedUrl, OAuthAccessToken oAuthAccessToken, Dictionary<string, string> parameters);
	}
}