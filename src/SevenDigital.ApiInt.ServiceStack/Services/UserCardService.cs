using System.Collections.Generic;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.ParameterDefinitions.Post;
using SevenDigital.Api.Schema.User.Payment;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class UserCardService : Service
	{
		private readonly IFluentApi<Cards> _cardsApi;
		private readonly IFluentApi<AddCard> _addCardApi;
		private readonly IFluentApi<DeleteCard> _deleteCardApi;

		public UserCardService(IFluentApi<Cards> cardsApi, IFluentApi<AddCard> addCardApi, IFluentApi<DeleteCard> deleteCardApi)
		{
			_cardsApi = cardsApi;
			_addCardApi = addCardApi;
			_deleteCardApi = deleteCardApi;
		}

		public List<Card> Get(CardRequest request)
		{
			var accessToken = this.TryGetOAuthAccessToken();
			var please = _cardsApi.ForUser(accessToken.Token, accessToken.Secret).Please();
			return please.UserCards;
		}

		public List<Card> Post(AddCardRequest request)
		{
			var accessToken = this.TryGetOAuthAccessToken();

			int issueNum;
			int.TryParse(request.IssueNumber, out issueNum);
			var card = new AddCardParameters
			{
				ExpiryDate = request.ExpiryDate,
				HolderName = request.HolderName,
				IssueNumber = issueNum,
				Number = request.Number,
				PostCode = request.PostCode,
				StartDate = request.StartDate,
				TwoLetterISORegionName = request.TwoLetterISORegionName,
				Type = request.Type,
				VerificationCode = request.VerificationCode
			};
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