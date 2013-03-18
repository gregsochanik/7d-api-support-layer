using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.Common.Web;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.Pricing;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.ApiInt.Basket;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.Purchasing;
using SevenDigital.ApiInt.ServiceStack.Model;
using SevenDigital.ApiInt.ServiceStack.Services;
using SevenDigital.ApiInt.TestData;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class FreePurchaseServiceTests
	{
		private const int PRICED_TRACK_ID = 1234;
		private IPurchaseItemMapper _mapper;
		private ICatalogue _catalogue;
		private static readonly Track _freeTrack = new Track { Price = new Price { Value = "0" } };
		private static readonly Track _pricedTrack = new Track { Id = PRICED_TRACK_ID, Price = new Price() };
		private PurchasedItem _expectedPurchasedItem;
		private IBasketHandler _basketHandler;

		[SetUp]
		public void SetUp()
		{
			_mapper = MockRepository.GenerateStub<IPurchaseItemMapper>();
			
			_basketHandler = MockRepository.GenerateStub<IBasketHandler>();

			_expectedPurchasedItem = new PurchasedItem();
			_mapper.Stub(x => x.Map(null, null)).IgnoreArguments().Return(_expectedPurchasedItem);

			_catalogue = MockRepository.GenerateStub<ICatalogue>();
			_catalogue.Stub(x => x.GetATrackWithPrice(null, 0)).IgnoreArguments().Return(_freeTrack);
			_catalogue.Stub(x => x.GetATrackWithPrice("GB",PRICED_TRACK_ID)).Return(_pricedTrack);
		}

		[Test]
		public void Throws_error_if_item_is_not_free()
		{
			var catalogue = MockRepository.GenerateStub<ICatalogue>();
			catalogue.Stub(x => x.GetATrackWithPrice(null, 0)).IgnoreArguments().Return(_pricedTrack);

			var freePurchaseService = new FreePurchaseService(_mapper, _basketHandler, catalogue)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			var freePurchaseRequest = new FreePurchaseRequest{CountryCode = "GB", Id=PRICED_TRACK_ID, Type = PurchaseType.track};

			var httpError = Assert.Throws<HttpError>(() => freePurchaseService.Post(freePurchaseRequest));
			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
			Assert.That(httpError.Message, Is.EqualTo(string.Format("This track is not free! {0}", _pricedTrack.Price.Status)));
			Assert.That(httpError.ErrorCode, Is.EqualTo("TrackNotFree"));
		}
	}
}