using System.Collections.Generic;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.ParameterDefinitions.Post;
using SevenDigital.Api.Schema.User.Payment;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiSupportLayer.ServiceStack.Mapping;
using SevenDigital.ApiSupportLayer.ServiceStack.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services
{
	public class UserCardService : Service
	{
		private readonly IFluentApi<Cards> _cardsApi;
		private readonly IFluentApi<AddCard> _addCardApi;
		private readonly IFluentApi<DeleteCard> _deleteCardApi;
		private readonly IMapper<AddCardRequest, AddCardParameters> _cardMapper;

		public UserCardService(IFluentApi<Cards> cardsApi, IFluentApi<AddCard> addCardApi, IFluentApi<DeleteCard> deleteCardApi, IMapper<AddCardRequest, AddCardParameters> cardMapper)
		{
			_cardsApi = cardsApi;
			_addCardApi = addCardApi;
			_deleteCardApi = deleteCardApi;
			_cardMapper = cardMapper;
		}

		public List<Card> Get(CardRequest request)
		{
			var accessToken = this.TryGetOAuthAccessToken();
			var apiResponse = _cardsApi.ForUser(accessToken.Token, accessToken.Secret).Please();

			if (apiResponse.UserCards.Count == 1)
			{
				apiResponse.UserCards[0].IsDefault = true;
			}

			return apiResponse.UserCards;
		}

		public List<Card> Post(AddCardRequest request)
		{
			var accessToken = this.TryGetOAuthAccessToken();

			var card = _cardMapper.Map(request);
			try
			{
				_addCardApi.ForUser(accessToken.Token, accessToken.Secret).WithCard(card).Please();
				return Get(request);
			}
			catch (ApiException ex)
			{
				throw new HttpError(HttpStatusCode.BadRequest, "404", ex.Message);
			}
		} 

		public List<Card> Delete(CardRequest request)
		{
			if (request.Id < 1)
				return Get(request);

			var accessToken = this.TryGetOAuthAccessToken();

			_deleteCardApi.ForUser(accessToken.Token, accessToken.Secret).WithCard(request.Id).Please();
			return Get(request);
		} 
	}
}