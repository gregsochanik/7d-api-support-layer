using System;
using System.Net;
using ServiceStack.Common.Web;
using SevenDigital.Api.Schema.Premium.Basket;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Wrapper.Premium;
using SevenDigital.ApiSupportLayer.Basket;
using SevenDigital.ApiSupportLayer.Mapping;
using SevenDigital.ApiSupportLayer.Model;
using SevenDigital.ApiSupportLayer.Purchasing;
using SevenDigital.ApiSupportLayer.ServiceStack.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services
{
	public class VoucherPurchaseService : BasketPurchaseService<VoucherPurchaseRequest>
	{
		private readonly IFluentApi<ApplyVoucherToBasket> _applyVoucher;

		public VoucherPurchaseService(IFluentApi<ApplyVoucherToBasket> applyVoucher, IPurchaseItemMapper mapper, IBasketHandler basketHandler)
			: base(mapper, basketHandler)
		{
			_applyVoucher = applyVoucher;
		}

		public PurchaseResponse Post(VoucherPurchaseRequest request)
		{
			if (string.IsNullOrEmpty(request.VoucherCode))
			{
				throw VoucherMissingException();
			}
			return RunBasketPurchaseSteps(request, PerformPaymentStep);
		}

		public void PerformPaymentStep(Guid basketId, VoucherPurchaseRequest request)
		{
			try
			{
				AssertVoucherApplied(() => _applyVoucher
					                           .UseBasketId(basketId)
					                           .UseVoucherCode(request.VoucherCode)
					                           .WithParameter("country", request.CountryCode)
					                           .Please(),
				                     request.Type);
			}
			catch (ApiErrorException)
			{
				throw VoucherInvalidException(request.Type);
			}
		}

		private static void AssertVoucherApplied(Func<ApplyVoucherToBasket> action, PurchaseType type)
		{
			var applyVoucherToBasket = action();

			if (applyVoucherToBasket.AmountDue != null
			    && double.Parse(applyVoucherToBasket.AmountDue.Amount) > 0
			    && applyVoucherToBasket.BasketItems.Items.Count < 2)
			{
				throw VoucherInvalidException(type);
			}
		}

		private static HttpError VoucherInvalidException(PurchaseType type)
		{
			return new HttpError(HttpStatusCode.BadRequest, "VoucherInvalid", string.Format("This voucher is not valid for {0}s", type.ToString().ToLower()));
		}

		private static HttpError VoucherMissingException()
		{
			return new HttpError(HttpStatusCode.BadRequest, "VoucherMissing", "You need to include a voucher code");
		}
	}
}