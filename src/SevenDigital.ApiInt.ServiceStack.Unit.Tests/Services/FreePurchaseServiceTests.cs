using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
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
		[Test]
		public void Adds_item_to_locker()
		{
			var itemBuyer = MockRepository.GenerateStub<IItemBuyer>();
			var expectedLockerReleases = new List<LockerRelease>();
			itemBuyer.Stub(x => x.BuyItem(null, null)).IgnoreArguments().Return(expectedLockerReleases);

			var mapper = MockRepository.GenerateStub<IPurchaseItemMapper>();
			var expectedPurchasedItem = new PurchasedItem();
			mapper.Stub(x => x.Map(null, null)).IgnoreArguments().Return(expectedPurchasedItem);

			var freePurchaseService = new FreePurchaseService(mapper, itemBuyer)
			{
				RequestContext = ContextHelper.LoggedInContext()
			}; 
			var freePurchaseRequest = new FreePurchaseRequest();
			var freePurchaseResponse = freePurchaseService.Get(freePurchaseRequest);

			itemBuyer.AssertWasCalled(x=>x.BuyItem(Arg<ItemRequest>.Is.Equal(freePurchaseRequest), Arg<OAuthAccessToken>.Is.Anything));

			Assert.That(freePurchaseResponse.Item, Is.EqualTo(expectedPurchasedItem));
			Assert.That(freePurchaseResponse.OriginalRequest, Is.EqualTo(freePurchaseRequest));
			Assert.That(freePurchaseResponse.Status.IsSuccess);
		}

		[Test]
		public void Uses_oauth_criteria()
		{
			var itemBuyer = MockRepository.GenerateStub<IItemBuyer>();
			itemBuyer.Stub(x => x.BuyItem(null, null)).IgnoreArguments().Return(new List<LockerRelease>());
			var mapper = MockRepository.GenerateStub<IPurchaseItemMapper>();

			var freePurchaseService = new FreePurchaseService(mapper, itemBuyer)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			var freePurchaseRequest = new FreePurchaseRequest();

			freePurchaseService.Get(freePurchaseRequest);

			itemBuyer.AssertWasCalled(x => x.BuyItem(Arg<ItemRequest>.Is.Equal(freePurchaseRequest),
				Arg<OAuthAccessToken>.Matches(accessToken => accessToken.Token == FakeUserData.FakeAccessToken.Token && accessToken.Secret == FakeUserData.FakeAccessToken.Secret)));
		}

		
	}
}