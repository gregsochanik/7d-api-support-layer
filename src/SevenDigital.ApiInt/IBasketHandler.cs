using System;
using SevenDigital.Api.Schema.Basket;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt
{
	public interface IBasketHandler
	{
		Guid Create(ItemRequest request);
		Basket AddItem(Guid basketId, ItemRequest request);
		UserPurchaseBasket Purchase(Guid basketId, string countryCode, OAuthAccessToken accessToken);
	}
}