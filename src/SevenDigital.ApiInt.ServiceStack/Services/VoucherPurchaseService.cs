using System;
using System.Collections.Generic;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.Basket;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.Premium.Basket;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Wrapper.Premium;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class VoucherPurchaseService : Service
	{
		private readonly IFluentApi<CreateBasket> _createBasket;
		private readonly IFluentApi<AddItemToBasket> _addItemToBasket;
		private readonly ICatalogue _catalogue;
		private readonly IFluentApi<ApplyVoucherToBasket> _applyVoucher;
		private readonly IFluentApi<UserPurchaseBasket> _purchaseBasket;
		private readonly IPurchaseItemMapper _mapper;

		public VoucherPurchaseService(IFluentApi<CreateBasket> createBasket, 
			IFluentApi<AddItemToBasket> addItemToBasket, 
			IFluentApi<ApplyVoucherToBasket> applyVoucher, 
			IFluentApi<UserPurchaseBasket> purchaseBasket, 
			ICatalogue catalogue, 
			IPurchaseItemMapper mapper)
		{
			_createBasket = createBasket;
			_addItemToBasket = addItemToBasket;
			_catalogue = catalogue;
			_applyVoucher = applyVoucher;
			_purchaseBasket = purchaseBasket;
			_mapper = mapper;
		}

		public VoucherPurchaseResponse Post(VoucherPurchaseRequest request)
		{
			// create a basket
			var createBasket = _createBasket.WithParameter("country", request.CountryCode).Please();
			var basketId = new Guid(createBasket.Id);

			// add item to backet
			_addItemToBasket.UseBasketId(basketId);
			AdjustApiCallBasedOnPurchaseType(_addItemToBasket, request);
			_addItemToBasket.WithParameter("country", request.CountryCode).Please();

			if (string.IsNullOrEmpty(request.VoucherCode))
				throw new HttpError("BOOM");

			// apply voucher to basket
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

			// purchase basket
			var accessToken = this.TryGetOAuthAccessToken();

			// handle failure
			var userPurchaseBasket = _purchaseBasket.ForUser(accessToken.Token, accessToken.Secret)
										.WithParameter("basketId", basketId.ToString())
										.WithParameter("country", request.CountryCode)
										.Please();

			return BuildVoucherPurchaseResponse(request, new PurchaseStatus(true, "Voucher worked", userPurchaseBasket.LockerReleases));
		}

		private void AdjustApiCallBasedOnPurchaseType(IFluentApi<AddItemToBasket> api, ItemRequest request)
		{
			if (request.Type == PurchaseType.release)
			{
				api.ForReleaseId(request.Id);
			}
			else
			{
				var track = _catalogue.GetATrack(request.CountryCode, request.Id);
				var releaseId = track.Release.Id;
				api.ForReleaseId(releaseId).ForTrackId(request.Id);
			}
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