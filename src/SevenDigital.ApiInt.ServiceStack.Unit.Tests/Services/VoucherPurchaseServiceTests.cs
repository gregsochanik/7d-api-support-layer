using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.Premium.Basket;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Premium;
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
			_userPurchaseBasket = MockRepository.GenerateStub<IFluentApi<UserPurchaseBasket>>();
			_purchaseItemMapper = MockRepository.GenerateStub<IPurchaseItemMapper>();

			_basketHandler = MockRepository.GenerateStub<IBasketHandler>();
			_basketHandler.Stub(x => x.Purchase(Guid.Empty, null, null)).IgnoreArguments().Return(new UserPurchaseBasket(){LockerReleases = new List<LockerRelease>(), PurchaseDate = DateTime.Now});

			_applyVoucher.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(_applyVoucher);
			_userPurchaseBasket.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(_userPurchaseBasket);
			_userPurchaseBasket.Stub(x => x.ForUser(null, null)).IgnoreArguments().Return(_userPurchaseBasket);
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
			Assert.That(voucherResponse.VoucherCode, Is.EqualTo(voucherPurchaseRequest.VoucherCode));
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
	}
}