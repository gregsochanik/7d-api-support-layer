using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.User;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Purchasing
{
	public class UserDeliverItemBuyer : IItemBuyer
	{
		private readonly IFluentApi<UserDeliverItem> _deliverItemApi;
		private readonly ICatalogue _catalogue;

		private const string TRANSACTION_ID = "7d-shop-purchase";

		public UserDeliverItemBuyer(IFluentApi<UserDeliverItem> deliverItemApi, ICatalogue catalogue)
		{
			_deliverItemApi = deliverItemApi;
			_catalogue = catalogue;
		}

		public IEnumerable<LockerRelease> BuyItem(ItemRequest request, OAuthAccessToken accessToken)
		{
			var api = _deliverItemApi.ForUser(accessToken.Token, accessToken.Secret)
										.WithTransactionId(TRANSACTION_ID)
										.WithParameter("country", request.CountryCode);
			
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

			var userDeliverItem = api.Please();
			return userDeliverItem.LockerReleases;
		}
	}
}