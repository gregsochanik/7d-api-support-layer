using System;
using System.Collections.Generic;
using System.Net;
using SevenDigital.ApiInt.Basket;
using SevenDigital.ApiInt.Exceptions;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public static class BasketRequestHelper
	{
		public static Guid TryRetrieveBasketId(ItemRequest request, IDictionary<string, Cookie> requestCookies, IBasketHandler basketHandler)
		{
			if (!requestCookies.Keys.Contains(StateHelper.BASKET_COOKIE_NAME))
			{
				return basketHandler.Create(request);
			}

			var basketIdFromCookie = requestCookies[StateHelper.BASKET_COOKIE_NAME].Value;

			try
			{
				return new Guid(basketIdFromCookie);
			}
			catch (FormatException formatException)
			{
				throw new InvalidBasketIdException(basketIdFromCookie, formatException);
			}
		}
	}
}