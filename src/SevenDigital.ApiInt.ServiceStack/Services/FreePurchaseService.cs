using System.Linq;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.Pricing;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.Purchasing;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class FreePurchaseService : Service
	{
		private readonly IPurchaseItemMapper _mapper;
		private readonly IItemBuyer _itemBuyer;
		private readonly ICatalogue _catalogue;

		public FreePurchaseService(IPurchaseItemMapper mapper, IItemBuyer itemBuyer, ICatalogue catalogue)
		{
			_mapper = mapper;
			_itemBuyer = itemBuyer;
			_catalogue = catalogue;
		}

		public FreePurchaseResponse Post(FreePurchaseRequest request)
		{
			AssertItemIsTrack(request);
			AssertItemIsFree(request);

			var accessToken = this.TryGetOAuthAccessToken();
			var lockerReleases = _itemBuyer.BuyItem(request, accessToken).ToList();

			return new FreePurchaseResponse
			{
				Item = _mapper.Map(request, lockerReleases),
				OriginalRequest = request,
				Status = new PurchaseStatus(true, "OK", lockerReleases)
			};
		}

		private static void AssertItemIsTrack(FreePurchaseRequest request)
		{
			if (request.Type == PurchaseType.release)
			{
				throw new HttpError(HttpStatusCode.Forbidden,
				                    "ReleasesNotSupported",
				                    "You cannot access releases for free - only tracks are currently supported");
			}
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