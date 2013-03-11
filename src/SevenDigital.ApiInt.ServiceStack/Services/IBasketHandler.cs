using System;
using SevenDigital.Api.Schema.Basket;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public interface IBasketHandler
	{
		Guid Create(ItemRequest request);
		Basket AddItem(Guid basketId, ItemRequest request);
	}
}