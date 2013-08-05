using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Purchasing.CardPurchaseRules
{
	public class ApiCardPurchaseRule : ICardPurchaseRule
	{
		private const string INVALID_CARD_MESSAGE = "Invalid card selected";
		private const string PURCHASE_ACCEPTED = "Purchase accepted";

		private readonly IFluentApi<UserPurchaseItem> _purchase;
		private readonly ICatalogue _catalogue;

		public ApiCardPurchaseRule(IFluentApi<UserPurchaseItem> purchase, ICatalogue catalogue)
		{
			_purchase = purchase;
			_catalogue = catalogue;
		}

		public PurchaseStatus FulfillPurchase(CardPurchaseRequest request, OAuthAccessToken accessToken)
		{
			if (request.CardId <= 0)
				return new PurchaseStatus(false, INVALID_CARD_MESSAGE, new List<LockerRelease>());
			
			var api = _purchase.ForUser(accessToken.Token, accessToken.Secret)
			                   .WithParameter("country", request.CountryCode)
							   .ForPrice(request.Price);

			AdjustApiCallBasedOnPurchaseType(api, request);

			return TryCardPurchase(api);
		}

		private static PurchaseStatus TryCardPurchase(IFluentApi<UserPurchaseItem> api)
		{
			try
			{
				var userDeliverItem = api.Please();
				return new PurchaseStatus(true, PURCHASE_ACCEPTED, userDeliverItem.LockerReleases);
			}
			catch (ApiException ex)
			{
				return new PurchaseStatus(false, ex.Message, new List<LockerRelease>());
			}
		}

		private void AdjustApiCallBasedOnPurchaseType(IFluentApi<UserPurchaseItem> api, ItemRequest request)
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
	}
}