using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.Premium.Basket;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Premium;
using SevenDigital.ApiInt.Basket;
using SevenDigital.ApiInt.Exceptions;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;
using SevenDigital.ApiInt.ServiceStack.Services;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class VoucherPurchaseServiceTests
	{
		private IFluentApi<ApplyVoucherToBasket> _applyVoucher;
		private IFluentApi<UserPurchaseBasket> _userPurchaseBasket;
		private IPurchaseItemMapper _purchaseItemMapper;
		private IBasketHandler _basketHandler;

		[SetUp]
		public void SetUp()
		{
			_applyVoucher = MockRepository.GenerateStub<IFluentApi<ApplyVoucherToBasket>>();
			_applyVoucher.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(_applyVoucher);
			_applyVoucher.Stub(x => x.Please()).Return(new ApplyVoucherToBasket());

			_userPurchaseBasket = MockRepository.GenerateStub<IFluentApi<UserPurchaseBasket>>();
			_userPurchaseBasket.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(_userPurchaseBasket);
			_userPurchaseBasket.Stub(x => x.ForUser(null, null)).IgnoreArguments().Return(_userPurchaseBasket);

			_purchaseItemMapper = MockRepository.GenerateStub<IPurchaseItemMapper>();

			_basketHandler = MockRepository.GenerateStub<IBasketHandler>();
			_basketHandler.Stub(x => x.Purchase(Guid.Empty, null, null)).IgnoreArguments().Return(new UserPurchaseBasket { LockerReleases = new List<LockerRelease>(), PurchaseDate = DateTime.Now });
			
			var lockerReleases = new List<LockerRelease>{new LockerRelease { Release = new Release(), LockerTracks = new List<LockerTrack>()}};
			_userPurchaseBasket.Stub(x => x.Please()).Return(new UserPurchaseBasket { LockerReleases = lockerReleases});

		}

		[Test]
		public void returns_cardvoucherresponse()
		{
			var voucherPurchaseRequest = new VoucherPurchaseRequest{VoucherCode="12345678"};
			var userVoucherService = new VoucherPurchaseService(_applyVoucher, _purchaseItemMapper, _basketHandler)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			var voucherResponse = userVoucherService.Post(voucherPurchaseRequest);
			Assert.That(voucherResponse, Is.Not.Null);
			Assert.That(voucherResponse.Status.Message, Is.EqualTo("Purchase Authorised"));
		}
		
		[Test]
		public void applys_voucher_to_that_basket()
		{
			const string expectedVoucherCode = "12345678";

			var cardVoucherRequest = new VoucherPurchaseRequest { VoucherCode = expectedVoucherCode, Type = PurchaseType.track };
			var userVoucherService = new VoucherPurchaseService(_applyVoucher, _purchaseItemMapper, _basketHandler)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			userVoucherService.Post(cardVoucherRequest);

			_applyVoucher.AssertWasCalled(x => x.UseVoucherCode(expectedVoucherCode));
		}

		[Test]
		public void if_basket_cookie_exists_use_this_basket()
		{
			const string expectedVoucherCode = "12345678";
			var fakeBasketId = Guid.NewGuid();

			var cardVoucherRequest = new VoucherPurchaseRequest { VoucherCode = expectedVoucherCode, Type = PurchaseType.track };
			var userVoucherService = new VoucherPurchaseService(_applyVoucher, _purchaseItemMapper, _basketHandler)
			{
				RequestContext = ContextHelper.LoggedInContextWithFakeBasketCookie(fakeBasketId)
			};

			userVoucherService.Post(cardVoucherRequest);

			_basketHandler.AssertWasNotCalled(x => x.Create(Arg<ItemRequest>.Is.Anything));
			_basketHandler.AssertWasCalled(x => x.AddItem(Arg<Guid>.Is.Equal(fakeBasketId), Arg<ItemRequest>.Is.Anything));
		}

		[Test]
		public void if_no_cookie_create_new_basket()
		{
			const string expectedVoucherCode = "12345678";

			var cardVoucherRequest = new VoucherPurchaseRequest { VoucherCode = expectedVoucherCode, Type = PurchaseType.track };
			var userVoucherService = new VoucherPurchaseService(_applyVoucher, _purchaseItemMapper, _basketHandler)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};

			userVoucherService.Post(cardVoucherRequest);

			_basketHandler.AssertWasCalled(x => x.Create(Arg<ItemRequest>.Is.Anything));
		}

		[Test]
		public void throws_correct_error_if_not_a_valid_guid()
		{
			const string expectedVoucherCode = "12345678";

			const string invalidBasketId = "I will not be made into a Guid!";
			var mockRequestContext = ContextHelper.LoggedInContext();
			mockRequestContext.Cookies.Add(StateHelper.BASKET_COOKIE_NAME, new Cookie(StateHelper.BASKET_COOKIE_NAME, invalidBasketId));

			var cardVoucherRequest = new VoucherPurchaseRequest { VoucherCode = expectedVoucherCode};
			var userVoucherService = new VoucherPurchaseService(_applyVoucher, _purchaseItemMapper, _basketHandler)
			{
				RequestContext = mockRequestContext
			};

			var invalidBasketIdException = Assert.Throws<InvalidBasketIdException>(() => userVoucherService.Post(cardVoucherRequest));

			Assert.That(invalidBasketIdException.Message, Is.EqualTo(string.Format("BasketId {0} is invalid", invalidBasketId)));
			Assert.That(invalidBasketIdException.InnerException, Is.TypeOf<FormatException>());
		}
	}
}