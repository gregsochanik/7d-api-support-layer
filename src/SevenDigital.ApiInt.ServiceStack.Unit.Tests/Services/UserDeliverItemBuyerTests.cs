using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Schema.User;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.Purchasing;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class UserDeliverItemBuyerTests
	{
		private IFluentApi<UserDeliverItem> _deliverItemApi;
		private ICatalogue _catalogue;

		[SetUp]
		public void SetUp()
		{
			_deliverItemApi = MockRepository.GenerateStub<IFluentApi<UserDeliverItem>>();
			_catalogue = MockRepository.GenerateStub<ICatalogue>();
			
		}

		[Test]
		public void Sets_releaseId_if_release_request()
		{
			var userDeliverItem = UserDeliverItem();

			var userDeliverItemBuyer = new UserDeliverItemBuyer(_deliverItemApi, _catalogue);

			var itemRequest = new ItemRequest{CountryCode = "UK", Id = 1234, Type = PurchaseType.release};
			var oAuthTokens = new OAuthAccessToken()
			{
				Token = "Token", Secret = "Secret"
			};
			var buyItem = userDeliverItemBuyer.BuyItem(itemRequest, oAuthTokens);

			Assert.That(buyItem, Is.EqualTo(userDeliverItem.LockerReleases));
			_deliverItemApi.AssertWasCalled(x => x.ForReleaseId(1234));
			_deliverItemApi.AssertWasCalled(x => x.WithParameter("country", itemRequest.CountryCode));
			_deliverItemApi.AssertWasNotCalled(x => x.ForTrackId(1234));
		}

		[Test]
		public void Sets_trackId_AND_releaseId_if_track_request()
		{
			var userDeliverItem = UserDeliverItem();
			_catalogue.Stub(x => x.GetATrack("", 0)).IgnoreArguments().Return(new Track
			{
				Release = new Release
				{
					Id = 1234
				}
			});

			var userDeliverItemBuyer = new UserDeliverItemBuyer(_deliverItemApi, _catalogue);

			var itemRequest = new ItemRequest { CountryCode = "UK", Id = 1234, Type = PurchaseType.track };
			var oAuthTokens = new OAuthAccessToken { Token = "Token", Secret = "Secret" };
			var buyItem = userDeliverItemBuyer.BuyItem(itemRequest, oAuthTokens);

			Assert.That(buyItem, Is.EqualTo(userDeliverItem.LockerReleases));
			_deliverItemApi.AssertWasCalled(x => x.ForReleaseId(1234));
			_deliverItemApi.AssertWasCalled(x => x.ForTrackId(1234));
			_deliverItemApi.AssertWasCalled(x => x.WithParameter("country", itemRequest.CountryCode));
		}

		private UserDeliverItem UserDeliverItem()
		{
			_deliverItemApi.Stub(x => x.ForUser(null, null)).IgnoreArguments().Return(_deliverItemApi);
			_deliverItemApi.Stub(x => x.ForReleaseId(0)).IgnoreArguments().Return(_deliverItemApi);
			_deliverItemApi.Stub(x => x.ForTrackId(0)).IgnoreArguments().Return(_deliverItemApi);
			_deliverItemApi.Stub(x => x.WithTransactionId(null)).IgnoreArguments().Return(_deliverItemApi);
			_deliverItemApi.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(_deliverItemApi);
			var userDeliverItem = new UserDeliverItem { LockerReleases = new List<LockerRelease>(), PurchaseDate = DateTime.Now };
			_deliverItemApi.Stub(x => x.Please()).Return(userDeliverItem);
			return userDeliverItem;
		}
	}
}