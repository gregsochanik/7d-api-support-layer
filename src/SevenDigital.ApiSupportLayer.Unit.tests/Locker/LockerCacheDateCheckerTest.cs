using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiSupportLayer.Locker;
using SevenDigital.ApiSupportLayer.TestData;
using SevenDigital.ApiSupportLayer.TestData.StubApiWrapper;

namespace SevenDigital.ApiSupportLayer.Unit.Tests.Locker
{
	[TestFixture]
	public class LockerCacheDateCheckerTest
	{
		private readonly OAuthAccessToken _oAuthAccessToken = new OAuthAccessToken
		{
			Secret = "secret",
			Token = "TOKEN"
		};

		[SetUp]
		public void Given_a_clearCache()
		{
			//InMemoryCacheStore.Instance.FlushAll();
		}

		[Test]
		public void Getting_purchase_date_of_live_locker_doesnt_break_if_no_items_in_locker()
		{
			var anEmptyLocker = new List<LockerRelease>();
			var locker = new Api.Schema.LockerEndpoint.Locker
			{
				Response = new LockerResponse { LockerReleases = anEmptyLocker }
			};

			var stubbedLockerApi = ApiWrapper.StubbedApi(locker);
			var lockerCacheDateChecker = new LockerDateChecker(stubbedLockerApi);
			var latestPurchaseDateOfLiveLocker = lockerCacheDateChecker.GetLatestPurchaseDateOfLiveLocker(_oAuthAccessToken);
			Assert.That(latestPurchaseDateOfLiveLocker, Is.EqualTo(DateTime.MinValue));
		}

		[Test]
		public void Getting_purchase_date_of_cached_locker_doesnt_break_if_no_items_in_locker()
		{
			var anEmptyLocker = new List<LockerRelease>();

			var lockerCache = MockRepository.GenerateStub<ILockerCacheAdapter>();
			lockerCache.Stub(x => x.GetCachedLocker(null)).IgnoreArguments().Return(anEmptyLocker);
			var latestPurchaseDateOfLiveLocker = lockerCache.GetLatestPurchaseDateOfCachedLocker(_oAuthAccessToken);
			Assert.That(latestPurchaseDateOfLiveLocker, Is.EqualTo(DateTime.MinValue));
		}

		[Test]
		public void Cached_locker_and_live_locker_should_be_ordered_the_same_no_matter_how_they_come_back_from_the_api()
		{
			var purchaseDates = new Dictionary<DateTime, string>
			{
				{new DateTime(2007, 11, 01, 10, 10, 10),"01 November 07"},
				{new DateTime(2010, 11, 01, 10, 10, 10),"01 November 10"},
				{new DateTime(2008, 11, 01, 10, 10, 10),"01 November 08"},
				{new DateTime(2011, 11, 01, 10, 10, 10),"01 November 11"},
				{new DateTime(2009, 11, 01, 10, 10, 10),"01 November 09"},
			};
			var lockerWithReleasesWithDifferentPurchaseDates = TestLocker.GetLockerWithReleasesWithDifferentPurchaseDates(purchaseDates);

			var stubbedLockerToReturn = new Api.Schema.LockerEndpoint.Locker { Response = lockerWithReleasesWithDifferentPurchaseDates };
			var stubbedLockerApi = ApiWrapper.StubbedApi(stubbedLockerToReturn);
			var lockerCacheDateChecker = new LockerDateChecker(stubbedLockerApi);
			var timedCacheReloading = MockRepository.GenerateStub<ITimedCacheReloading>();
			timedCacheReloading.Stub(x => x.TimedSynchronousCacheGet<IEnumerable<LockerRelease>>(null, "", 0)).IgnoreArguments().Return(lockerWithReleasesWithDifferentPurchaseDates.LockerReleases);

			var userTokenCache = MockRepository.GenerateStub<IUserTokenCache>();
			var lockerCache = new LockerCacheAdapter(timedCacheReloading, userTokenCache);
			var latestPurchaseDateOfLiveLocker = lockerCacheDateChecker.GetLatestPurchaseDateOfLiveLocker(FakeUserData.FakeAccessToken);
			var latestPurchaseDateOfCachedLocker = lockerCache.GetLatestPurchaseDateOfCachedLocker(FakeUserData.FakeAccessToken);

			Assert.That(latestPurchaseDateOfCachedLocker, Is.EqualTo(latestPurchaseDateOfLiveLocker));
		}
	}
}