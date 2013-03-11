using System;
using System.Net;
using ServiceStack.Common.Web;
using SevenDigital.Api.Schema.Premium.Basket;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Premium;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class VoucherPurchaseService : BasketPurchaseService<VoucherPurchaseRequest>
	{
		private readonly IFluentApi<ApplyVoucherToBasket> _applyVoucher;
		
		public VoucherPurchaseService(IFluentApi<ApplyVoucherToBasket> applyVoucher, IPurchaseItemMapper mapper, IBasketHandler basketHandler)
			:base(mapper, basketHandler)
		{
			_applyVoucher = applyVoucher;
		}

		public PurchaseResponse Post(VoucherPurchaseRequest request)
		{
			if (string.IsNullOrEmpty(request.VoucherCode))
			{
				throw new HttpError(HttpStatusCode.BadRequest, "VoucherMissing", "You need to include a voucher code");
			}

			return RunBasketPurchaseSteps(request);
		}

		public override void PerformPaymentStep(Guid basketId, VoucherPurchaseRequest request)
		{
			_applyVoucher.UseBasketId(basketId).UseVoucherCode(request.VoucherCode)
							 .WithParameter("country", request.CountryCode)
							 .Please();
		}
	}
}