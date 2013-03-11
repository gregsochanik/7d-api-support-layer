using System;
using System.Collections.Generic;
using System.Net;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiInt.Exceptions;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public abstract class BasketPurchaseService<TItemRequest> : Service where TItemRequest : ItemRequest
	{
		private readonly IPurchaseItemMapper _mapper;
		private readonly IBasketHandler _basketHandler;

		protected BasketPurchaseService(IPurchaseItemMapper mapper, IBasketHandler basketHandler)
		{
			_mapper = mapper;
			_basketHandler = basketHandler;
		}

		public PurchaseResponse RunBasketPurchaseSteps(TItemRequest request, Action<Guid, TItemRequest> paymentStep)
		{
			var basketId = TryRetrieveBasketId(request, RequestContext.Cookies);

			_basketHandler.AddItem(basketId, request);

			try
			{
				paymentStep(basketId, request);

				var apiBasketPurchaseResponse = _basketHandler.Purchase(basketId, request.CountryCode, this.TryGetOAuthAccessToken());

				return PurchaseSuccessfulResponse(request, apiBasketPurchaseResponse);
			}
			catch (ApiException ex)
			{
				return ApiErrorResponse(request, ex);
			}
		}

		private Guid TryRetrieveBasketId(ItemRequest request, IDictionary<string, Cookie> requestCookies)
		{
			if (!requestCookies.Keys.Contains(StateHelper.BASKET_COOKIE))
			{
				return _basketHandler.Create(request);
			}

			var basketIdFromCookie = requestCookies[StateHelper.BASKET_COOKIE].Value;

			try
			{
				return new Guid(basketIdFromCookie);
			}
			catch (FormatException formatException)
			{
				throw new InvalidBasketIdException(basketIdFromCookie, formatException);
			}
		}

		private PurchaseResponse PurchaseSuccessfulResponse(ItemRequest request, UserPurchaseBasket userPurchaseBasket)
		{
			return new PurchaseResponse
			{
				OriginalRequest = request,
				Status = new PurchaseStatus(true, "Purchase Authorised", userPurchaseBasket.LockerReleases),
				Item = _mapper.Map(request, userPurchaseBasket.LockerReleases)
			};
		}

		private static PurchaseResponse ApiErrorResponse(ItemRequest request, ApiException ex)
		{
			return new PurchaseResponse
			{
				OriginalRequest = request,
				Status = new PurchaseStatus(false, ex.Message, new List<LockerRelease>()),
			};
		}
	}
}