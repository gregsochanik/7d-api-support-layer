using System.Collections.Generic;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.User.Payment;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.Purchasing;
using SevenDigital.ApiInt.Purchasing.CardPurchaseRules;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class CardPurchaseService : Service
	{
		private readonly IPurchaseItemMapper _mapper;
		private readonly ICardPurchaseRule _cardPurchaseRule;
		private readonly IPriceWatch _priceWatch;
		private readonly IFluentApi<DefaultCard> _cardApi;

		private readonly ILog _logger = LogManager.GetLogger("CardPurchaseService");

		public CardPurchaseService(IPurchaseItemMapper mapper, ICardPurchaseRule cardPurchaseRule, IPriceWatch priceWatch, IFluentApi<DefaultCard> cardApi)
		{
			_mapper = mapper;
			_cardPurchaseRule = cardPurchaseRule;
			_priceWatch = priceWatch;
			_cardApi = cardApi;
		}

		public CardPurchaseResponse Post(CardPurchaseRequest request)
		{
			if (string.IsNullOrEmpty(request.CountryCode))
				request.CountryCode = "GB";

			request.Price = _priceWatch.GetItemPrice(request);

			var accessToken = this.TryGetOAuthAccessToken();

			if (!TrySetDefaultCard(accessToken, request.CardId))
			{
				return BuildFailedCardPurchasedResponse(request, string.Format("Could not set default card to {0}", request.CardId));
			}
			
			var cardPurchaseStatus = _cardPurchaseRule.FulfillPurchase(request, accessToken);

			return BuildCardPurchaseResponse(request, cardPurchaseStatus);
		}

		private static CardPurchaseResponse BuildFailedCardPurchasedResponse(CardPurchaseRequest request, string failureMessage)
		{
			var failedPurchaseStatus = new PurchaseStatus(false, failureMessage, new List<LockerRelease>());
			return new CardPurchaseResponse { Item = null, OriginalRequest = request, Status = failedPurchaseStatus };
		}

		private CardPurchaseResponse BuildCardPurchaseResponse(CardPurchaseRequest request, PurchaseStatus purchaseStatus)
		{
			var cardPurchaseResponse = new CardPurchaseResponse
			{
				OriginalRequest = request,
				Status = purchaseStatus,
			};

			if (cardPurchaseResponse.Status.IsSuccess)
				cardPurchaseResponse.Item = _mapper.Map(request, purchaseStatus.UpdatedLocker);

			return cardPurchaseResponse;
		}

		private bool TrySetDefaultCard(OAuthAccessToken accessToken, int cardId)
		{
			try
			{
				_cardApi.ForUser(accessToken.Token, accessToken.Secret).WithCard(cardId).Please();
				return true;
			}
			catch(ApiException ex)
			{
				_logger.Error(string.Format("Failed to set default card to {0}", cardId), ex);
				return false;
			}
		}
	}
}