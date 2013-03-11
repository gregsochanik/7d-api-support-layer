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
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class VoucherPurchaseService : Service
	{
		private readonly IFluentApi<ApplyVoucherToBasket> _applyVoucher;
		private readonly IFluentApi<UserPurchaseBasket> _purchaseBasket;
		private readonly IPurchaseItemMapper _mapper;
		private readonly IBasketHandler _basketHandler;

		public VoucherPurchaseService(IFluentApi<ApplyVoucherToBasket> applyVoucher, IFluentApi<UserPurchaseBasket> purchaseBasket, IPurchaseItemMapper mapper, IBasketHandler basketHandler)
		{
			_applyVoucher = applyVoucher;
			_purchaseBasket = purchaseBasket;
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
				_applyVoucher.UseBasketId(basketId).UseVoucherCode(request.VoucherCode)
				             .WithParameter("country", request.CountryCode)
				             .Please();
			}
			catch (ApiException ex)
			{
				return BuildVoucherPurchaseResponse(request, new PurchaseStatus(false, ex.Message, new List<LockerRelease>()));
			}

			var accessToken = this.TryGetOAuthAccessToken();

			var userPurchaseBasket = _purchaseBasket.ForUser(accessToken.Token, accessToken.Secret)
										.WithParameter("basketId", basketId.ToString())
										.WithParameter("country", request.CountryCode)
										.Please();

			return BuildVoucherPurchaseResponse(request, new PurchaseStatus(true, "Voucher worked", userPurchaseBasket.LockerReleases));
		}

		private VoucherPurchaseResponse BuildVoucherPurchaseResponse(VoucherPurchaseRequest request, PurchaseStatus purchaseStatus)
		{
			var voucherPurchaseResponse = new VoucherPurchaseResponse
			{
				OriginalRequest = request,
				Status = purchaseStatus,
				VoucherCode = request.VoucherCode,
			};

			if (voucherPurchaseResponse.Status.IsSuccess)
				voucherPurchaseResponse.Item = _mapper.Map(request, purchaseStatus.UpdatedLocker);

			return voucherPurchaseResponse;
		}
	}
}