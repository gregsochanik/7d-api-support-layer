using System.Net;
using ServiceStack.Common.Web;
using SevenDigital.Api.Schema.Pricing;
using SevenDigital.ApiSupportLayer.Basket;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.Mapping;
using SevenDigital.ApiSupportLayer.Model;
using SevenDigital.ApiSupportLayer.Purchasing;
using SevenDigital.ApiSupportLayer.ServiceStack.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services
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
			if (request.Type == PurchaseType.track)
			{

				var aTrackWithPrice = _catalogue.GetATrackWithPrice(request.CountryCode, request.Id);
				if (aTrackWithPrice.Price.Status != PriceStatus.Free)
				{
					throw new HttpError(HttpStatusCode.Forbidden,
					                    "TrackNotFree",
					                    string.Format("This track is not free! {0}", aTrackWithPrice.Price.Status));
				}
			}
			else
			{
				var release = _catalogue.GetARelease(request.CountryCode, request.Id);
				if (release.Price.Status != PriceStatus.Free)
				{
					throw new HttpError(HttpStatusCode.Forbidden,
					                    "ReleaseNotFree",
					                    string.Format("This release is not free! {0}", release.Price.Status));
				}
			}
		}
	}
}