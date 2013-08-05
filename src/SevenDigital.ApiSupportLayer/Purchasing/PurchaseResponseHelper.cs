using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiSupportLayer.Mapping;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Purchasing
{
	public static class PurchaseResponseHelper
	{
		public static PurchaseResponse PurchaseSuccessfulResponse(ItemRequest request, UserPurchaseBasket userPurchaseBasket, IPurchaseItemMapper mapper)
		{
			return new PurchaseResponse
			{
				OriginalRequest = request,
				Status = new PurchaseStatus(true, "Purchase Authorised", userPurchaseBasket.LockerReleases),
				Item = mapper.Map(request, userPurchaseBasket.LockerReleases)
			};
		}

		public static PurchaseResponse ApiErrorResponse(ItemRequest request, ApiException ex)
		{
			return new PurchaseResponse
			{
				OriginalRequest = request,
				Status = new PurchaseStatus(false, ex.Message, new List<LockerRelease>()),
			};
		}
	}
}