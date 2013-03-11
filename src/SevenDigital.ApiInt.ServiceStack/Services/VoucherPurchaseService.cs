using System;
using System.Collections.Generic;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.Premium.Basket;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Wrapper.Premium;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class VoucherPurchaseService : Service
	{
		private readonly IFluentApi<ApplyVoucherToBasket> _applyVoucher;
		private readonly IPurchaseItemMapper _mapper;
		private readonly IBasketHandler _basketHandler;

		public VoucherPurchaseService(IFluentApi<ApplyVoucherToBasket> applyVoucher, IPurchaseItemMapper mapper, IBasketHandler basketHandler)
		{
			_applyVoucher = applyVoucher;
			_mapper = mapper;
			_basketHandler = basketHandler;
		}

		public VoucherPurchaseResponse Post(VoucherPurchaseRequest request)
		{
			var basketId = _basketHandler.Create(request);
			_basketHandler.AddItem(basketId, request);

			if (string.IsNullOrEmpty(request.VoucherCode))
				throw new HttpError(HttpStatusCode.BadRequest, "VoucherMissing", "You need to include a voucher code");

			try
			{
				ApplyVoucherToBasket(request, basketId);
			}
			catch (ApiException ex)
			{
				return ApiErrorResponse(request, ex);
			}

			return PurchaseBasket(request, basketId, this.TryGetOAuthAccessToken());
		}

		private void ApplyVoucherToBasket(VoucherPurchaseRequest request, Guid basketId)
		{
			_applyVoucher.UseBasketId(basketId).UseVoucherCode(request.VoucherCode)
			             .WithParameter("country", request.CountryCode)
			             .Please();
		}

		private VoucherPurchaseResponse PurchaseBasket(VoucherPurchaseRequest request, Guid basketId, OAuthAccessToken accessToken)
		{
			var userPurchaseBasket = _basketHandler.Purchase(basketId, request.CountryCode, accessToken);

			return new VoucherPurchaseResponse
			{
				OriginalRequest = request,
				Status = new PurchaseStatus(true, "Voucher worked", userPurchaseBasket.LockerReleases),
				VoucherCode = request.VoucherCode,
				Item = _mapper.Map(request, userPurchaseBasket.LockerReleases)
			};
		}

		private static VoucherPurchaseResponse ApiErrorResponse(VoucherPurchaseRequest request, ApiException ex)
		{
			return new VoucherPurchaseResponse
			{
				OriginalRequest = request,
				Status = new PurchaseStatus(false, ex.Message, new List<LockerRelease>()),
				VoucherCode = request.VoucherCode
			};
		}
	}
}