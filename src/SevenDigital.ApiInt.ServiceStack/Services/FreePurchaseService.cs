using System.Net;
using ServiceStack.Common.Web;
using SevenDigital.Api.Schema.Pricing;
using SevenDigital.ApiInt.Basket;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.Purchasing;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class FreePurchaseService : BasketPurchaseService<FreePurchaseRequest>
	{
		private readonly ICatalogue _catalogue;

		public FreePurchaseService(IPurchaseItemMapper mapper, IBasketHandler basketHandler, ICatalogue catalogue)
			: base(mapper, basketHandler)
		{
			_catalogue = catalogue;
		}

		public PurchaseResponse Post(FreePurchaseRequest request)
		{
			AssertItemIsFree(request);

			return RunBasketPurchaseSteps(request);
		}

		private void AssertItemIsFree(ItemRequest request)
		{
			var aTrackWithPrice = _catalogue.GetATrackWithPrice(request.CountryCode, request.Id);
			if (aTrackWithPrice.Price.Status != PriceStatus.Free)
			{
				throw new HttpError(HttpStatusCode.Forbidden,
									"TrackNotFree",
									string.Format("This track is not free! {0}", aTrackWithPrice.Price.Status));
			}
		}
	}
}