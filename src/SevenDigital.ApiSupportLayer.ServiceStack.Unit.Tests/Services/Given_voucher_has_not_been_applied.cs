using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.Common.Web;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Schema.Basket;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.Premium.Basket;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Wrapper.Http;
using SevenDigital.ApiSupportLayer.Basket;
using SevenDigital.ApiSupportLayer.Mapping;
using SevenDigital.ApiSupportLayer.Model;
using SevenDigital.ApiSupportLayer.ServiceStack.Model;
using SevenDigital.ApiSupportLayer.ServiceStack.Services;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	[Description("BUYNOW-76")]
	public class Given_voucher_has_not_been_applied
	{
		private IPurchaseItemMapper _purchaseItemMapper;
		private IBasketHandler _basketHandler;

		[SetUp]
		public void SetUp()
		{
			_purchaseItemMapper = MockRepository.GenerateStub<IPurchaseItemMapper>();
			_basketHandler = MockRepository.GenerateStub<IBasketHandler>();
			_basketHandler.Stub(x => x.Purchase(Guid.Empty, null, null)).IgnoreArguments().Return(new UserPurchaseBasket { LockerReleases = new List<LockerRelease>(), PurchaseDate = DateTime.Now });
		}

		[Test]
		[Description("BUYNOW-77")]
		public void then_if_basket_has_a_value_gt_zero_it_throws_expected_error()
		{
			var applyVoucher = MockRepository.GenerateStub<IFluentApi<ApplyVoucherToBasket>>();
			applyVoucher.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(applyVoucher);
			applyVoucher.Stub(x => x.Please()).Return(new ApplyVoucherToBasket { AmountDue = new AmountDue { Amount = "0.99", FormattedAmount = "£0.99" }, BasketItems = new BasketItemList { Items = new List<BasketItem> { new BasketItem() } } });

			var basketId = Guid.NewGuid().ToString();

			var mockRequestContext = ContextHelper.LoggedInContext();
			mockRequestContext.Cookies.Add(StateHelper.BASKET_COOKIE_NAME, new Cookie(StateHelper.BASKET_COOKIE_NAME, basketId));

			var voucherRequest = new VoucherPurchaseRequest { VoucherCode = "12345678", Type = PurchaseType.release };
			var userVoucherService = new VoucherPurchaseService(applyVoucher, _purchaseItemMapper, _basketHandler)
			{
				RequestContext = mockRequestContext
			};

			var httpError = Assert.Throws<HttpError>(() => userVoucherService.Post(voucherRequest));

			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
			Assert.That(httpError.ErrorCode, Is.EqualTo("VoucherInvalid"));
			Assert.That(httpError.Message, Is.EqualTo("This voucher is not valid for releases"));
		}

		[Test]
		[Description("BUYNOW-77")]
		public void then_if_basket_has_a_value_gt_zero_but_more_than_one_basketItem_it_does_not_throw_expected_error()
		{
			var applyVoucher = MockRepository.GenerateStub<IFluentApi<ApplyVoucherToBasket>>();
			applyVoucher.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(applyVoucher);
			applyVoucher.Stub(x => x.Please()).Return(new ApplyVoucherToBasket { AmountDue = new AmountDue { Amount = "0.99", FormattedAmount = "£0.99" }, BasketItems = new BasketItemList { Items = new List<BasketItem> { new BasketItem(), new BasketItem() } } });

			var basketId = Guid.NewGuid().ToString();

			var mockRequestContext = ContextHelper.LoggedInContext();
			mockRequestContext.Cookies.Add(StateHelper.BASKET_COOKIE_NAME, new Cookie(StateHelper.BASKET_COOKIE_NAME, basketId));

			var voucherRequest = new VoucherPurchaseRequest { VoucherCode = "12345678", Type = PurchaseType.release };
			var userVoucherService = new VoucherPurchaseService(applyVoucher, _purchaseItemMapper, _basketHandler)
			{
				RequestContext = mockRequestContext
			};

			Assert.DoesNotThrow(() => userVoucherService.Post(voucherRequest));
		}


		[Test]
		[Description("BUYNOW-78")]
		public void then_if_apply_voucher_returns_3012_it_throws_expected_error()
		{
			var applyVoucher = MockRepository.GenerateStub<IFluentApi<ApplyVoucherToBasket>>();
			applyVoucher.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(applyVoucher);
			applyVoucher.Stub(x => x.Please()).Throw(new DummyApiException("", new Response(HttpStatusCode.BadRequest, "blah"), ErrorCode.AddCardDeclinedError));

			var basketId = Guid.NewGuid().ToString();

			var mockRequestContext = ContextHelper.LoggedInContext();
			mockRequestContext.Cookies.Add(StateHelper.BASKET_COOKIE_NAME, new Cookie(StateHelper.BASKET_COOKIE_NAME, basketId));

			var voucherRequest = new VoucherPurchaseRequest { VoucherCode = "12345678", Type = PurchaseType.release };
			var userVoucherService = new VoucherPurchaseService(applyVoucher, _purchaseItemMapper, _basketHandler)
			{
				RequestContext = mockRequestContext
			};

			var httpError = Assert.Throws<HttpError>(() => userVoucherService.Post(voucherRequest));

			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
			Assert.That(httpError.ErrorCode, Is.EqualTo("VoucherInvalid"));
			Assert.That(httpError.Message, Is.EqualTo("This voucher is not valid for releases"));
		}

		public class DummyApiException : ApiErrorException
		{
			public DummyApiException(string message, Response response, ErrorCode errorCode)
				: base(message, response, errorCode)
			{ }

			public DummyApiException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{ }
		}
	}
}