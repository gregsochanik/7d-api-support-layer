using System;
using System.Configuration;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiSupportLayer.Basket;
using SevenDigital.ApiSupportLayer.Mapping;
using SevenDigital.ApiSupportLayer.Model;
using SevenDigital.ApiSupportLayer.Purchasing;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services
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

				var purchaseData = new PurchaseData
				{
					CountryCode = request.CountryCode,
					SalesTagName = ConfigurationManager.AppSettings["BasketPurchase.TagName"],
					SalesTagValue = ConfigurationManager.AppSettings["BasketPurchase.TagValue"]
				};

				var apiBasketPurchaseResponse = _basketHandler.Purchase(basketId, purchaseData, this.TryGetOAuthAccessToken());
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