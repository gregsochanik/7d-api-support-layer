using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiSupportLayer.TestData
{
	public static class FakeUserData
	{
		public static OAuthAccessToken FakeAccessToken
		{
			get { return new OAuthAccessToken { Token = "TOKEN", Secret = "SECRET" }; }
		}

		public static OAuthAccessToken RealAccessToken
		{
			get { return new OAuthAccessToken { Token = "FtjhXIBcKCltoLAUrcmHKw==", Secret = "+sTxiCB91j5NE8wX0cA==" }; }
		}
	}
}