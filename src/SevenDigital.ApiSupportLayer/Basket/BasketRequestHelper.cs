using System;
using System.Collections.Generic;
using System.Net;
using SevenDigital.ApiSupportLayer.Exceptions;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Basket
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