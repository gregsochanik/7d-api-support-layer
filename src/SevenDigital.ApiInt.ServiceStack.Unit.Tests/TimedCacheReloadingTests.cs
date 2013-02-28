using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.CacheAccess;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiInt.Locker;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests
{
	
	public class TestClass
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	[TestFixture]
	public class TimedCacheReloadingTests
	{
		[Test]
		public void Attempts_to_trigger_cache_reload_if_not_already_started()
		{
			var cacheClient = MockRepository.GenerateStub<ICacheClient>();
			var lockerReloader = MockRepository.GenerateStub<ILockerReloader>();
			var timedCacheReloading = new TimedCacheReloading(cacheClient, lockerReloader);

			const string key = "myKey";
			const int timeout = 1;
			timedCacheReloading.TimedSynchronousCacheGet<TestClass>(new OAuthAccessToken(), key, timeout);
			lockerReloader.AssertWasCalled(x => x.FullLockerCacheRefreshAsync(Arg<OAuthAccessToken>.Is.Anything));
		}

		[Test]
		public void Returns_item_if_is_there_in_time()
		{
			var cacheClient = MockRepository.GenerateStub<ICacheClient>();
			var timedCacheReloading = new TimedCacheReloading(cacheClient, null);

			const string key = "myKey";
			const int timeout = 1;

			cacheClient.Stub(x => x.Get<TestClass>(key)).Return(new TestClass
			{
				Id = 1,
				Name = "Red Socks Pugie"
			});

			var timedSynchronousCacheGet = timedCacheReloading.TimedSynchronousCacheGet<TestClass>(null, key, timeout);

			Assert.That(timedSynchronousCacheGet.Id, Is.EqualTo(1));
			Assert.That(timedSynchronousCacheGet.Name, Is.EqualTo("Red Socks Pugie"));
		}
	}
}