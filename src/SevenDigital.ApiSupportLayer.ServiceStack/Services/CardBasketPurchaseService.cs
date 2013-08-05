using System.Collections.Generic;
using ServiceStack.Logging;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.User.Payment;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiInt.Basket;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.Purchasing;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class CardBasketPurchaseService : BasketPurchaseService<CardBasketPurchaseRequest>
	{
		private readonly IFluentApi<DefaultCard> _cardApi;

		private readonly ILog _logger = LogManager.GetLogger("CardBasketPurchaseService");

		public CardBasketPurchaseService(IPurchaseItemMapper mapper, IFluentApi<DefaultCard> cardApi, IBasketHandler basketHandler)
			: base(mapper, basketHandler)
		{
			_cardApi = cardApi;
		}

		public PurchaseResponse Post(CardBasketPurchaseRequest request)
		{
			if (string.IsNullOrEmpty(request.CountryCode))
				request.CountryCode = "GB";

			var accessToken = this.TryGetOAuthAccessToken();

			if (!TrySetDefaultCard(accessToken, request.CardId))
			{
				return BuildFailedCardPurchasedResponse(request, string.Format("Could not set default card to {0}", request.CardId));
			}

			return RunBasketPurchaseSteps(request);
		}

		private bool TrySetDefaultCard(OAuthAccessToken accessToken, int cardId)
		{
			try
			{
				_cardApi.ForUser(accessToken.Token, accessToken.Secret).WithCard(cardId).Please();
				return true;
			}
			catch (ApiException ex)
			{
				_logger.Error(string.Format("Failed to set default card to {0}", cardId), ex);
				return false;
			}
		}

		private static PurchaseResponse BuildFailedCardPurchasedResponse(ItemRequest request, string failureMessage)
		{
			var failedPurchaseStatus = new PurchaseStatus(false, failureMessage, new List<LockerRelease>());
			return new PurchaseResponse { Item = null, OriginalRequest = request, Status = failedPurchaseStatus };
		}
	}
}