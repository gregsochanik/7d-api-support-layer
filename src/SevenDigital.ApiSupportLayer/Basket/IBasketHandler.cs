using System;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Basket
{
	public interface IBasketHandler
	{
		Guid Create(ItemRequest request);
		Api.Schema.Basket.Basket AddItem(Guid basketId, ItemRequest request);
		UserPurchaseBasket Purchase(Guid basketId, PurchaseData countryCode, OAuthAccessToken accessToken);
	}
}