using System;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Basket
{
	public interface IBasketHandler
	{
		Guid Create(ItemRequest request);
		Api.Schema.Basket.Basket AddItem(Guid basketId, ItemRequest request);
		UserPurchaseBasket Purchase(Guid basketId, PurchaseData countryCode, OAuthAccessToken accessToken);
	}
}