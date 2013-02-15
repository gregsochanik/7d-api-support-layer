using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface.Testing;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.User.Payment;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Wrapper.Http;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.Purchasing;
using SevenDigital.ApiInt.Purchasing.CardPurchaseRules;
using SevenDigital.ApiInt.ServiceStack.Model;
using SevenDigital.ApiInt.ServiceStack.Services;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class CardPurchaseServiceTests
	{
		private IPurchaseItemMapper _purchaseItemMapper;
		private ICardPurchaseRule _cardPurchaseRule;
		private IItemBuyer _itemBuyer;
		private IPriceWatch _priceWatch;
		private IFluentApi<DefaultCard> _defaultCardApi;

		[SetUp]
		public void SetUp()
		{
			_purchaseItemMapper = MockRepository.GenerateStub<IPurchaseItemMapper>();
			_itemBuyer = MockRepository.GenerateStub<IItemBuyer>();
			_cardPurchaseRule = new StubCardPurchaseRule(_itemBuyer);
			_priceWatch = MockRepository.GenerateStub<IPriceWatch>();
			_defaultCardApi = MockRepository.GenerateStub<IFluentApi<DefaultCard>>();
			_defaultCardApi.Stub(x => x.WithParameter("", "")).IgnoreArguments().Return(_defaultCardApi);
			_defaultCardApi.Stub(x => x.ForUser("", "")).IgnoreArguments().Return(_defaultCardApi);
			_defaultCardApi.Stub(x => x.WithCard(0)).IgnoreArguments().Return(_defaultCardApi);
		}

		[Test]
		public void Happy_path()
		{
			var objToReturn = new PurchasedItem();
			_purchaseItemMapper.Stub(x => x.Map(null, null)).IgnoreArguments().Return(objToReturn);
			_itemBuyer.Stub(x => x.BuyItem(null, null)).IgnoreArguments().Return(new List<LockerRelease>());

			var cardPurchaseService = new CardPurchaseService(_purchaseItemMapper, _cardPurchaseRule, _priceWatch, _defaultCardApi)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};

			var cardPurchaseRequest = new CardPurchaseRequest {CardId = 1};
			var cardPurchaseResponse = cardPurchaseService.Post(cardPurchaseRequest);

			Assert.That(cardPurchaseResponse.OriginalRequest, Is.EqualTo(cardPurchaseRequest));
			Assert.That(cardPurchaseResponse.Item, Is.EqualTo(objToReturn));
		}

		[Test]
		public void Returns_expected_on_payment_success()
		{
			var objToReturn = new PurchasedItem();
			_purchaseItemMapper.Stub(x => x.Map(null, null)).IgnoreArguments().Return(objToReturn);
			_itemBuyer.Stub(x => x.BuyItem(null, null)).IgnoreArguments().Return(new List<LockerRelease>() );

			var cardPurchaseService = new CardPurchaseService(_purchaseItemMapper, _cardPurchaseRule, _priceWatch, _defaultCardApi)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};

			var cardPurchaseRequest = new CardPurchaseRequest { CardId = 1 }; 
			var cardPurchaseResponse = cardPurchaseService.Post(cardPurchaseRequest);

			Assert.That(cardPurchaseResponse.Status.IsSuccess);
		}

		[Test]
		public void Returns_expected_on_payment_failure()
		{
			var cardPurchaseService = new CardPurchaseService(_purchaseItemMapper, _cardPurchaseRule, _priceWatch, _defaultCardApi)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};

			var cardPurchaseRequest = new CardPurchaseRequest { CardId = 2 };
			var cardPurchaseResponse = cardPurchaseService.Post(cardPurchaseRequest);

			Assert.That(cardPurchaseResponse.Status.IsSuccess, Is.False);

		}

		[Test]
		public void Throws_error_if_no_user_logged_in()
		{
			var cardService = new CardPurchaseService(_purchaseItemMapper, _cardPurchaseRule, _priceWatch, _defaultCardApi);
			var mockRequestContext = new MockRequestContext();
			cardService.RequestContext = mockRequestContext;

			var httpError = Assert.Throws<HttpError>(() => cardService.Post(new CardPurchaseRequest()));

			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
		}

		[Test]
		[Ignore("Pending BUYNOW-20 - problem with Request Tokens on api wrapper POST")]
		public void Handles_inability_to_set_default_card()
		{
			var cardService = new CardPurchaseService(_purchaseItemMapper, _cardPurchaseRule, _priceWatch, _defaultCardApi)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};

			_defaultCardApi.Stub(x => x.Please()).Throw(new RemoteApiException("blah", new Response(HttpStatusCode.NotFound, "blah"), ErrorCode.UserHasNoCardDetails));

			CardPurchaseResponse cardPurchaseResponse = null;
			Assert.DoesNotThrow(() => cardPurchaseResponse = cardService.Post(new CardPurchaseRequest{CardId = 123}));
			Assert.That(cardPurchaseResponse, Is.Not.Null);
			Assert.That(cardPurchaseResponse.Status.IsSuccess, Is.False);
			Assert.That(cardPurchaseResponse.Status.Message, Is.EqualTo("Could not set default card to 123"));
		}
	}
}