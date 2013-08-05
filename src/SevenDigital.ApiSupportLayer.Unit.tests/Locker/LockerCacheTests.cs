using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiSupportLayer.Locker;
using SevenDigital.ApiSupportLayer.TestData;

namespace SevenDigital.ApiSupportLayer.Unit.Tests.Locker
{
	[TestFixture]
	public class LockerCacheTests
	{
		private static readonly Dictionary<DateTime, string> _purchaseDates = new Dictionary<DateTime, string>
		{
			{new DateTime(2012, 1, 1), "Recent"},
			{new DateTime(2010, 10, 1), "Oct"},
			{new DateTime(2010, 11, 1), "Nov"},
			{new DateTime(2010, 12, 1), "Dec"},
		};

		private readonly LockerResponse _lockerWithReleaseWithDefinedPurchaseDates = TestLocker.GetLockerWithReleasesWithDifferentPurchaseDates(_purchaseDates);
		private readonly LockerResponse _lockerWithDifferentArtists = TestLocker.GetLockerWithReleaseBy2DifferentArtists(5);
		private LockerCacheAdapter _lockerCache;

		private ITimedCacheReloading _timedCacheReloading;

		private readonly OAuthAccessToken _oAuthAccessToken = new OAuthAccessToken
		{
			Secret = "secret",
			Token = "TOKEN"
		};

		private LockerCacheAdapter _secondLockerCache;
		
		[SetUp]
		public void Given_a_locker_api_that_returns_a_locker()
		{
			_timedCacheReloading = MockRepository.GenerateStub<ITimedCacheReloading>();
			_timedCacheReloading.Stub(x => x.TimedSynchronousCacheGet<IEnumerable<LockerRelease>>(null, "", 0)).IgnoreArguments().Return(_lockerWithReleaseWithDefinedPurchaseDates.LockerReleases);
			_timedCacheReloading.Stub(x => x.TimedSynchronousCacheGet<LockerStats>(null, "", 0)).IgnoreArguments().Return(new LockerStats(_lockerWithReleaseWithDefinedPurchaseDates.LockerReleases.Count));
			var userTokenCache = MockRepository.GenerateStub<IUserTokenCache>();
			_lockerCache = new LockerCacheAdapter(_timedCacheReloading, userTokenCache);
			_secondLockerCache = new LockerCacheAdapter(_timedCacheReloading, userTokenCache);
		}

		[Test]
		public void Second_GetLocker_call_comes_from_cache()
		{
			var fromCache = _lockerCache.GetCachedLocker(_oAuthAccessToken);
			var fromCache2 = _secondLockerCache.GetCachedLocker(_oAuthAccessToken);

			Assert.That(fromCache, Is.EqualTo(fromCache2));

		}

		[Test]
		public void Can_get_a_grouped_list_of_artists()
		{
			const int expectedNumberOfArtists = 2;

			var timedCacheReloading = MockRepository.GenerateStub<ITimedCacheReloading>();
			timedCacheReloading.Stub(x => x.TimedSynchronousCacheGet<IEnumerable<LockerRelease>>(null, "", 0)).IgnoreArguments().Return(_lockerWithDifferentArtists.LockerReleases);

			var userTokenCache = MockRepository.GenerateStub<IUserTokenCache>();
			var cache = new LockerCacheAdapter(timedCacheReloading, userTokenCache);

			var lockerArtists = cache.GetLockerArtists(_oAuthAccessToken).ToList();

			Assert.That(lockerArtists.Count(), Is.EqualTo(expectedNumberOfArtists));
			Assert.That(lockerArtists[0].Name, Is.EqualTo("Fleet Foxes"));
			Assert.That(lockerArtists[1].Name, Is.EqualTo("Keane"));

		}
	}
}