using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.Testing;
using SevenDigital.Api.Schema.User.Payment;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.ServiceStack.Mapping;
using SevenDigital.ApiInt.ServiceStack.Model;
using SevenDigital.ApiInt.ServiceStack.Services;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class UserCardServiceTests
	{
		private IFluentApi<Cards> _cardsApi;
		private Cards _cardsToReturn;
		private IFluentApi<AddCard> _addCardApi;
		private IFluentApi<DeleteCard> _deleteCardApi;

		[SetUp]
		public void SetUp()
		{
			_cardsToReturn = new Cards { UserCards = new List<Card> { new Card() } };
			_addCardApi = MockRepository.GenerateStub<IFluentApi<AddCard>>();

			_cardsApi = GetStubbedTrackApi(_cardsToReturn);
			_deleteCardApi = MockRepository.GenerateStub<IFluentApi<DeleteCard>>();
		}

		[Test]
		public void Happy_path()
		{
			var cardService = new UserCardService(_cardsApi, _addCardApi, _deleteCardApi, new AddCardMapper());

			var mockRequestContext = new MockRequestContext();
			var httpReq = mockRequestContext.Get<IHttpRequest>();
			var httpRes = mockRequestContext.Get<IHttpResponse>();
			var authUserSession = mockRequestContext.ReloadSession();
			authUserSession.Id = httpRes.CreateSessionId(httpReq);
			authUserSession.IsAuthenticated = true;
			authUserSession.ProviderOAuthAccess = new List<IOAuthTokens> { new OAuthTokens { AccessToken = "Token", AccessTokenSecret = "Secret" } };

			httpReq.Items[ServiceExtensions.RequestItemsSessionKey] = authUserSession;
			cardService.RequestContext = mockRequestContext;

			var cards = cardService.Get(new CardRequest());

			Assert.That(cards.Count, Is.EqualTo(_cardsToReturn.UserCards.Count));
		}

		[Test]
		public void Throws_error_if_no_user_logged_in()
		{
			var cardService = new UserCardService(_cardsApi, _addCardApi, _deleteCardApi, new AddCardMapper());
			var mockRequestContext = new MockRequestContext();
			cardService.RequestContext = mockRequestContext;

			var httpError = Assert.Throws<HttpError>(() => cardService.Get(new CardRequest()));

			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
		}

		public static IFluentApi<Cards> GetStubbedTrackApi(Cards cardsToReturn)
		{
			var fluentApi = MockRepository.GenerateStub<IFluentApi<Cards>>();
			fluentApi.Stub(x => x.WithParameter("", "")).IgnoreArguments().Return(fluentApi);
			fluentApi.Stub(x => x.ForUser(null, null)).IgnoreArguments().Return(fluentApi);
			fluentApi.Stub(x => x.Please()).Return(cardsToReturn);
			return fluentApi;
		}
	}
}