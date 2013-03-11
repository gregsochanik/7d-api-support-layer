using System;
using System.Collections.Generic;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.Premium.Basket;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Wrapper.Premium;
using SevenDigital.ApiInt.Exceptions;
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
			var basketId = TryRetrieveBasketId(request, RequestContext.Cookies);

			_basketHandler.AddItem(basketId, request);

			if (string.IsNullOrEmpty(request.VoucherCode))
			{
				throw new HttpError(HttpStatusCode.BadRequest, "VoucherMissing", "You need to include a voucher code");
			}

			try
			{
				_applyVoucher.UseBasketId(basketId).UseVoucherCode(request.VoucherCode)
				             .WithParameter("country", request.CountryCode)
				             .Please();

				var apiBasketPurchaseResponse = _basketHandler.Purchase(basketId, request.CountryCode, this.TryGetOAuthAccessToken());
				
				return SuccessResponse(request, apiBasketPurchaseResponse);
			}
			catch (ApiException ex)
			{
				return ApiErrorResponse(request, ex);
			}
		}

		private Guid TryRetrieveBasketId(ItemRequest request, IDictionary<string, Cookie> requestCookies)
		{
			if (!requestCookies.Keys.Contains(StateHelper.BASKET_COOKIE))
			{
				return _basketHandler.Create(request);
			}

			var basketIdFromCookie = requestCookies[StateHelper.BASKET_COOKIE].Value;

			try
			{
				return new Guid(basketIdFromCookie);
			}
			catch (FormatException formatException)
			{
				throw new InvalidBasketIdException(basketIdFromCookie, formatException);
			}
		}

		private VoucherPurchaseResponse SuccessResponse(VoucherPurchaseRequest request, UserPurchaseBasket userPurchaseBasket)
		{
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