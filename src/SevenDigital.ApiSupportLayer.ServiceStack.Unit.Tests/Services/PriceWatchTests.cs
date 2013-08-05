using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.Pricing;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.Purchasing;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class PriceWatchTests
	{
		private ICatalogue _catalogue;

		[SetUp]
		public void SetUp()
		{
			_catalogue = MockRepository.GenerateStub<ICatalogue>();
		}

		[Test]
		public void Can_get_release_price()
		{
			var objToReturn = new Release {Price = new Price { Rrp = "7.99", Value="7.99"}};

			_catalogue.Stub(x => x.GetARelease(null, 0)).IgnoreArguments().Return(objToReturn);

			var priceWatch = new PriceWatch(_catalogue);

			var releasePrice = priceWatch.GetReleasePrice("GB", 1);
			Assert.That(releasePrice, Is.EqualTo(7.99m));
		}

		[Test]
		public void Can_get_track_price()
		{
			var objToReturn = new Track { Price = new Price { Rrp = "0.99", Value = "0.99" } };

			_catalogue.Stub(x => x.GetATrackWithPrice(null, 0)).IgnoreArguments().Return(objToReturn);

			var priceWatch = new PriceWatch(_catalogue);

			var releasePrice = priceWatch.GetTrackPrice("GB", 1);
			Assert.That(releasePrice, Is.EqualTo(0.99m));
		}

		[Test]
		public void Can_get_release_price_as_itemprice()
		{
			var objToReturn = new Release { Price = new Price { Rrp = "7.99", Value = "7.99" } };

			_catalogue.Stub(x => x.GetARelease(null, 0)).IgnoreArguments().Return(objToReturn);

			var priceWatch = new PriceWatch(_catalogue);

			var itemRequest = new ItemRequest { CountryCode = "GB", Id = 1, Type = PurchaseType.release };
			var releasePrice = priceWatch.GetItemPrice(itemRequest);
			Assert.That(releasePrice, Is.EqualTo(7.99m));
		}

		[Test]
		public void Can_get_track_price_as_itemprice()
		{
			var objToReturn = new Track { Price = new Price { Rrp = "0.99", Value = "0.99" } };

			_catalogue.Stub(x => x.GetATrackWithPrice(null, 0)).IgnoreArguments().Return(objToReturn);

			var priceWatch = new PriceWatch(_catalogue);

			var itemRequest = new ItemRequest {CountryCode = "GB", Id= 1, Type = PurchaseType.track};
			var releasePrice = priceWatch.GetItemPrice(itemRequest);
			Assert.That(releasePrice, Is.EqualTo(0.99m));
		}

		[Test]
		public void Free_track_is_0_priced()
		{
			var objToReturn = new Track { Price = new Price { Rrp = "0.99", Value = "0" } };

			_catalogue.Stub(x => x.GetATrackWithPrice(null, 0)).IgnoreArguments().Return(objToReturn);

			var priceWatch = new PriceWatch(_catalogue);

			var itemRequest = new ItemRequest { CountryCode = "GB", Id = 1, Type = PurchaseType.track };
			var releasePrice = priceWatch.GetItemPrice(itemRequest);
			Assert.That(releasePrice, Is.EqualTo(0.00m));
		}
	}
}