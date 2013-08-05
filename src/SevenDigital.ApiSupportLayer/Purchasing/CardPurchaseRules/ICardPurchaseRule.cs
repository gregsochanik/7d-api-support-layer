using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Purchasing.CardPurchaseRules
{
	public interface ICardPurchaseRule
	{
		PurchaseStatus FulfillPurchase(CardPurchaseRequest request, OAuthAccessToken accessToken);
	}
}