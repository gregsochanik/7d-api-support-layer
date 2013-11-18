using NUnit.Framework;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiSupportLayer.Catalogue;

namespace SevenDigital.ApiSupportLayer.Unit.Tests.Catalogue
{
	[TestFixture]
	public class CacheKeysTest
	{
		private readonly OAuthAccessToken _oAuthAccessToken = new OAuthAccessToken
		{
			Token = "TOKEN",
			Secret = "SECRET"
		};

		private const string USERNAME = "test";
		private const int RELEASE_ID = 12345;

		[Test]
		public void Track_key_is_as_expected()
		{
			var cacheKey = CacheKeys.Track("GB", 12345);
			Assert.That(cacheKey, Is.EqualTo("urn:Track:GB:12345"));
		}

		[Test]
		public void Release_key_is_as_expected()
		{
			var cacheKey = CacheKeys.Release("GB", RELEASE_ID);
			Assert.That(cacheKey, Is.EqualTo("urn:Release:GB:" + RELEASE_ID));
		}

		[Test]
		public void ReleaseTracks_key_is_as_expected()
		{
			var cacheKey = CacheKeys.ReleaseTracks("GB", RELEASE_ID);
			Assert.That(cacheKey, Is.EqualTo("urn:ReleaseTracks:GB:" + RELEASE_ID));
		}

		[Test]
		public void Locker_key_is_as_expected()
		{
			var cacheKey = CacheKeys.LockerCacheKey(USERNAME);
			Assert.That(cacheKey, Is.EqualTo("urn:Locker:" + USERNAME));
		}

		[Test]
		public void LockerTotalItems_key_is_as_expected()
		{
			var cacheKey = CacheKeys.LockerTotalItemsCacheKey(USERNAME);
			Assert.That(cacheKey, Is.EqualTo("urn:Locker:Total:" + USERNAME));
		}

		[Test]
		public void OAuth_key_is_as_expected()
		{
			var cacheKey = CacheKeys.OAuthCacheKey(USERNAME);
			Assert.That(cacheKey, Is.EqualTo("urn:OAuth:" + USERNAME));
		}

		[Test]
		public void UnlimitedStreaming_key_is_as_expected()
		{
			var cacheKey = CacheKeys.UnlimitedStreamingCacheKey(USERNAME);
			Assert.That(cacheKey, Is.EqualTo("urn:Stream:" + USERNAME));
		}

		[Test]
		public void UserTokenMapping_key_is_as_expected()
		{
			var cacheKey = CacheKeys.UserTokenMappingCacheKey(_oAuthAccessToken);
			Assert.That(cacheKey, Is.EqualTo("urn:UserToken:" + _oAuthAccessToken.Token));
		}
	}
}
