using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Purchasing.CardPurchaseRules
{
	public class StubCardPurchaseRule : ICardPurchaseRule
	{
		private readonly IItemBuyer _buyer;
		private const string PAYMENT_ACCEPTED = "Payment accepted - 12345";
		private const string PAYMENT_DECLINED = "Payment declined - 12345";

		public StubCardPurchaseRule(IItemBuyer buyer)
		{
			_buyer = buyer;
		}

		public PurchaseStatus FulfillPurchase(CardPurchaseRequest request, OAuthAccessToken accessToken)
		{
			var isSuccess = HardCodedToIdOfOne(request);
			var message = isSuccess ? PAYMENT_ACCEPTED : PAYMENT_DECLINED;

			var lockerReleases = isSuccess ? _buyer.BuyItem(request, accessToken) : new List<LockerRelease>();

			return new PurchaseStatus(isSuccess, message, lockerReleases);	
		}

		private static bool HardCodedToIdOfOne(CardPurchaseRequest request)
		{
			return request.CardId == 1;
		}
	}
}