using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiInt.Catalogue
{
	internal static class CacheKeys
	{
		private const string UNLIMITED_STREAMING = "urn:stream:";
		private const string OAUTH = "urn:oauth:";
		private const string LOCKER_TOTALITEMS = "urn:locker:total:";
		private const string LOCKER = "urn:locker:";
		private const string USER_TOKEN_MAPPING = "urn:usertoken:";

		private const string TRACK = "urn:Track:{0}:{1}";
		private const string RELEASE = "urn:Release:{0}:{1}";
		private const string RELEASETRACKS = "urn:ReleaseTracks:{0}:{1}";

		public static string Track(string countryCode, int id)
		{
			return string.Format(TRACK, countryCode, id);
		}

		public static string Release(string countryCode, int id)
		{
			return string.Format(RELEASE, countryCode, id);
		}

		public static string ReleaseTracks(string countryCode, int id)
		{
			return string.Format(RELEASETRACKS, countryCode, id);
		}

		public static string LockerCacheKey(string username)
		{
			return string.Format("{0}{1}", LOCKER, username);
		}

		public static string LockerTotalItemsCacheKey(string username)
		{
			return string.Format("{0}{1}", LOCKER_TOTALITEMS, username);
		}
		
		public static string OAuthCacheKey(string username)
		{
			return string.Format("{0}{1}", OAUTH, username);
		}

		public static string UnlimitedStreamingCacheKey(string username)
		{
			return string.Format("{0}{1}", UNLIMITED_STREAMING, username);
		}

		public static string UserTokenMappingCacheKey(OAuthAccessToken accessToken)
		{
			return string.Format("{0}{1}", USER_TOKEN_MAPPING, accessToken.Token);

		}
	}
}