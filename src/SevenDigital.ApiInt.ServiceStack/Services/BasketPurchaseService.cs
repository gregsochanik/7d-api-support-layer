using System;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiInt.Basket;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.Purchasing;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public abstract class BasketPurchaseService<TItemRequest> : Service where TItemRequest : ItemRequest
	{
		private readonly IPurchaseItemMapper _mapper;
		private readonly IBasketHandler _basketHandler;
		private readonly ILog _logger = LogManager.GetLogger("BasketPurchaseService");

		protected BasketPurchaseService(IPurchaseItemMapper mapper, IBasketHandler basketHandler)
		{
			_mapper = mapper;
			_basketHandler = basketHandler;
		}

		public PurchaseResponse RunBasketPurchaseSteps(TItemRequest request, Action<Guid, TItemRequest> paymentStep = null)
		{
			var basketId = BasketRequestHelper.TryRetrieveBasketId(request, RequestContext.Cookies, _basketHandler);

			_basketHandler.AddItem(basketId, request);

			try
			{
				if (paymentStep != null)
				{
					paymentStep(basketId, request);
				}

				var apiBasketPurchaseResponse = _basketHandler.Purchase(basketId, request.CountryCode, this.TryGetOAuthAccessToken());
				_logger.InfoFormat("Item {0} purchased successfully - type {1} {2}", request.Id, request.Type, request.GetType().Name);
				return PurchaseResponseHelper.PurchaseSuccessfulResponse(request, apiBasketPurchaseResponse, _mapper);
			}
			catch (ApiException ex)
			{
				return PurchaseResponseHelper.ApiErrorResponse(request, ex);
			}
		}
	}
}