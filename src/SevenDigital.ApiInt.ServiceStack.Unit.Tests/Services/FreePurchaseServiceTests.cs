using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.Common.Web;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.Pricing;
using SevenDigital.Api.Schema.TrackEndpoint;
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
		private IItemBuyer _itemBuyer;
		private IPurchaseItemMapper _mapper;
		private ICatalogue _catalogue;
		private static readonly Track _freeTrack = new Track { Price = new Price { Value = "0" } };
		private static readonly Track _pricedTrack = new Track { Id = PRICED_TRACK_ID, Price = new Price() };
		private PurchasedItem _expectedPurchasedItem;
		private List<LockerRelease> _expectedLockerReleases;

		[SetUp]
		public void SetUp()
		{
			_itemBuyer = MockRepository.GenerateStub<IItemBuyer>();
			_mapper = MockRepository.GenerateStub<IPurchaseItemMapper>();

			_expectedLockerReleases = new List<LockerRelease>();
			_itemBuyer.Stub(x => x.BuyItem(null, null)).IgnoreArguments().Return(_expectedLockerReleases);

			_expectedPurchasedItem = new PurchasedItem();
			_mapper.Stub(x => x.Map(null, null)).IgnoreArguments().Return(_expectedPurchasedItem);

			_catalogue = MockRepository.GenerateStub<ICatalogue>();
			_catalogue.Stub(x => x.GetATrackWithPrice(null, 0)).IgnoreArguments().Return(_freeTrack);
			_catalogue.Stub(x => x.GetATrackWithPrice("GB",PRICED_TRACK_ID)).Return(_pricedTrack);
		}

		[Test]
		public void Adds_item_to_locker()
		{
			var freePurchaseService = new FreePurchaseService(_mapper, _itemBuyer, _catalogue)
			{
				RequestContext = ContextHelper.LoggedInContext()
			}; 
			var freePurchaseRequest = new FreePurchaseRequest();
			var freePurchaseResponse = freePurchaseService.Post(freePurchaseRequest);

			_itemBuyer.AssertWasCalled(x => x.BuyItem(Arg<ItemRequest>.Is.Equal(freePurchaseRequest), Arg<OAuthAccessToken>.Is.Anything));

			Assert.That(freePurchaseResponse.Item, Is.EqualTo(_expectedPurchasedItem));
			Assert.That(freePurchaseResponse.OriginalRequest, Is.EqualTo(freePurchaseRequest));
			Assert.That(freePurchaseResponse.Status.IsSuccess);
		}

		[Test]
		public void Uses_oauth_criteria()
		{
			var freePurchaseService = new FreePurchaseService(_mapper, _itemBuyer, _catalogue)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			var freePurchaseRequest = new FreePurchaseRequest();

			freePurchaseService.Post(freePurchaseRequest);

			_itemBuyer.AssertWasCalled(x => x.BuyItem(
				Arg<ItemRequest>.Is.Equal(freePurchaseRequest),
				Arg<OAuthAccessToken>.Matches(accessToken => accessToken.Token == FakeUserData.FakeAccessToken.Token && 
					accessToken.Secret == FakeUserData.FakeAccessToken.Secret))
					);
		}

		[Test]
		public void Throws_error_if_item_is_not_free()
		{
			var catalogue = MockRepository.GenerateStub<ICatalogue>();
			catalogue.Stub(x => x.GetATrackWithPrice(null, 0)).IgnoreArguments().Return(_pricedTrack);

			var freePurchaseService = new FreePurchaseService(_mapper, _itemBuyer, catalogue)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			var freePurchaseRequest = new FreePurchaseRequest{CountryCode = "GB", Id=PRICED_TRACK_ID, Type = PurchaseType.track};

			var httpError = Assert.Throws<HttpError>(() => freePurchaseService.Post(freePurchaseRequest));
			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
			Assert.That(httpError.Message, Is.EqualTo(string.Format("This track is not free! {0}", _pricedTrack.Price.Status)));
			Assert.That(httpError.ErrorCode, Is.EqualTo("TrackNotFree"));
		}

		[Test]
		[Ignore("BUYNOW-54: Releases can also be free")]
		public void Throws_error_if_item_is_release()
		{
			var catalogue = MockRepository.GenerateStub<ICatalogue>();

			var freePurchaseService = new FreePurchaseService(_mapper, _itemBuyer, catalogue)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			var freePurchaseRequest = new FreePurchaseRequest {Type = PurchaseType.release };

			var httpError = Assert.Throws<HttpError>(() => freePurchaseService.Post(freePurchaseRequest));
			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
			Assert.That(httpError.Message, Is.EqualTo("You cannot access releases for free - only tracks are currently supported"));
			Assert.That(httpError.ErrorCode, Is.EqualTo("ReleasesNotSupported"));
		}
	}
}