using System;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface.Testing;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Schema.ParameterDefinitions.Post;
using SevenDigital.Api.Schema.User.Payment;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Wrapper.Http;
using SevenDigital.ApiInt.ServiceStack.Mapping;
using SevenDigital.ApiInt.ServiceStack.Model;
using SevenDigital.ApiInt.ServiceStack.Services;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class UserCardServiceAddTests
	{
		private IFluentApi<Cards> _cardsApi;
		private IFluentApi<AddCard> _addCardApi;
		private IFluentApi<DeleteCard> _deleteCardApi;
		private IMapper<AddCardRequest, AddCardParameters> _mapper;

		[SetUp]
		public void SetUp()
		{
			_addCardApi = GetStubbedApi();
			_cardsApi = MockRepository.GenerateStub<IFluentApi<Cards>>();
			_deleteCardApi = MockRepository.GenerateStub<IFluentApi<DeleteCard>>();
			_mapper = new AddCardMapper();
		}

		[Test]
		public void Throws_error_if_no_user_logged_in()
		{
			var cardService = new UserCardService(_cardsApi, _addCardApi, _deleteCardApi, _mapper);
			var mockRequestContext = new MockRequestContext();
			cardService.RequestContext = mockRequestContext;

			var httpError = Assert.Throws<HttpError>(() => cardService.Post(new AddCardRequest()));

			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
		}


		[Test]
		public void Throws_error_on_api_error()
		{
			_addCardApi.Stub(x => x.Please()).Throw(new RemoteApiException("bad card brother", new Response(HttpStatusCode.NotFound, ""),  ErrorCode.AddCardFailedError));
			var cardService = new UserCardService(_cardsApi, _addCardApi, _deleteCardApi, _mapper);

			var mockRequestContext = ContextHelper.LoggedInContext();
			cardService.RequestContext = mockRequestContext;

			var httpError = Assert.Throws<HttpError>(() => cardService.Post(new AddCardRequest()));

			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
		}
		
		public static IFluentApi<AddCard> GetStubbedApi()
		{
			var fluentApi = MockRepository.GenerateStub<IFluentApi<AddCard>>();
			fluentApi.Stub(x => x.WithParameter("", "")).IgnoreArguments().Return(fluentApi);
			fluentApi.Stub(x => x.ForUser(null, null)).IgnoreArguments().Return(fluentApi);
			return fluentApi;
		}
	}
}