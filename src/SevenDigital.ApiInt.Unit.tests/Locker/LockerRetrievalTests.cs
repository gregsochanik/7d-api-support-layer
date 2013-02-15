using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.Locker;

namespace SevenDigital.ApiInt.Unit.Tests.Locker
{
	[TestFixture]
	public class LockerRetrievalTests
	{
		private readonly OAuthAccessToken _oAuthAccessToken = new OAuthAccessToken
		{
			Secret = "secret",
			Token = "TOKEN"
		};

		[Test]
		public void Sould_return_empty_list_if_totals_items_emtpy()
		{
			var apiLocker = MockRepository.GenerateStub<IFluentApi<Api.Schema.LockerEndpoint.Locker>>();

			var lockerRetrieval = new LockerRetrieval(apiLocker);
			var lockerReleases = lockerRetrieval.GetLockerReleases(_oAuthAccessToken, new LockerStats(0));
			Assert.That(lockerReleases, Is.Empty);
		}

		[Test]
		public void Sould_return_empty_list_if_totals_items_emtpy_minus()
		{
			var apiLocker = MockRepository.GenerateStub<IFluentApi<Api.Schema.LockerEndpoint.Locker>>();

			var lockerRetrieval = new LockerRetrieval(apiLocker);
			var lockerReleases = lockerRetrieval.GetLockerReleases(_oAuthAccessToken, new LockerStats(-10));
			Assert.That(lockerReleases, Is.Empty);
		}
	}

	public class TestClass
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
