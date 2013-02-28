using System;
using ServiceStack.CacheAccess;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Locker;

namespace SevenDigital.ApiInt.ServiceStack
{
	public class UserTokenCache : IUserTokenCache
	{
		private readonly ICacheClient _cacheClient;

		public UserTokenCache(ICacheClient cacheClient)
		{
			_cacheClient = cacheClient;
		}

		public string GetUsernameForToken(OAuthAccessToken token)
		{
			if (token == null)
				throw new ArgumentNullException("token");
			if (string.IsNullOrEmpty(token.Token))
				throw new ArgumentNullException("token");

			var key = CacheKeys.UserTokenMappingCacheKey(token);

			return _cacheClient.Get<string>(key);
		}

		public void SetUsernameForToken(OAuthAccessToken token, string username)
		{
			if (token == null)
				throw new ArgumentNullException("token");
			if (string.IsNullOrEmpty(token.Token))
				throw new ArgumentNullException("token");
			if (string.IsNullOrEmpty(username))
				throw new ArgumentNullException("username");

			var key = CacheKeys.UserTokenMappingCacheKey(token);
			_cacheClient.Set(key, username);
		}
	}
}