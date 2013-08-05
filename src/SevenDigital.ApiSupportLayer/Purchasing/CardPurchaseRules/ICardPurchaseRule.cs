using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Purchasing.CardPurchaseRules
{
	public interface ICardPurchaseRule
	{
		PurchaseStatus FulfillPurchase(CardPurchaseRequest request, OAuthAccessToken accessToken);
	}
}