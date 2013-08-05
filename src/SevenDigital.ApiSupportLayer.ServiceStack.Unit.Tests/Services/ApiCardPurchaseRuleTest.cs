using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.Api.Wrapper.Http;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.Model;
using SevenDigital.ApiSupportLayer.Purchasing.CardPurchaseRules;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class ApiCardPurchaseRuleTest
	{
		private IFluentApi<UserPurchaseItem> _stubPurchaseApi;
		private ICatalogue _stubCatalogueApi;

		[SetUp]
		public void SetUp()
		{
			_stubPurchaseApi = GetStubbed();
			_stubCatalogueApi = MockRepository.GenerateStub<ICatalogue>();
		}

		[Test]
		public void Succesful_track_Purchase_returns_correct_response()
		{
			_stubCatalogueApi.Stub(x => x.GetATrack(null, 0)).IgnoreArguments().Return(new Track()
			{
				Release = new Release
				{
					Id = 1
				}
			});

			var expectedLocker = new List<LockerRelease>() { new LockerRelease() { Release = new Release() { Title = "my test release" } } };
			_stubPurchaseApi.Stub(x => x.Please()).Return(new UserPurchaseItem()
			{
				LockerReleases = expectedLocker
			});

			var apiCardPurchaseRule = new ApiCardPurchaseRule(_stubPurchaseApi, _stubCatalogueApi);

			var cardPurchaseRequest = new CardPurchaseRequest{CardId = 1};
			var oAuthAccessToken = new OAuthAccessToken();

			var cardPurchaseStatus = apiCardPurchaseRule.FulfillPurchase(cardPurchaseRequest, oAuthAccessToken);

			Assert.That(cardPurchaseStatus.IsSuccess);
			Assert.That(cardPurchaseStatus.Message, Is.EqualTo("Purchase accepted"));
			Assert.That(cardPurchaseStatus.UpdatedLocker, Is.EqualTo(expectedLocker));
		}


		[Test]
		public void Succesful_release_Purchase_returns_correct_response()
		{
			_stubCatalogueApi.Stub(x => x.GetARelease(null, 0)).IgnoreArguments().Return(new Release
			{
					Id = 1
			});

			_stubCatalogueApi.Stub(x => x.GetAReleaseTracks(null, 0)).IgnoreArguments().Return(new List<Track>
			{
				new Track
				{
					Id=1
				}
			});

			var expectedLocker = new List<LockerRelease>() { new LockerRelease() { Release = new Release{ Title = "my test release" } } };
			_stubPurchaseApi.Stub(x => x.Please()).Return(new UserPurchaseItem()
			{
				LockerReleases = expectedLocker
			});

			var apiCardPurchaseRule = new ApiCardPurchaseRule(_stubPurchaseApi, _stubCatalogueApi);

			var cardPurchaseRequest = new CardPurchaseRequest { CardId = 1, Type = PurchaseType.release };
			var oAuthAccessToken = new OAuthAccessToken();

			var cardPurchaseStatus = apiCardPurchaseRule.FulfillPurchase(cardPurchaseRequest, oAuthAccessToken);

			Assert.That(cardPurchaseStatus.IsSuccess);
			Assert.That(cardPurchaseStatus.Message, Is.EqualTo("Purchase accepted"));
			Assert.That(cardPurchaseStatus.UpdatedLocker, Is.EqualTo(expectedLocker));
		}


		[Test]
		public void Error_purchase_returns_unsuccesful_response()
		{
			const string expectedErrorMessage = "Payment declined";

			_stubPurchaseApi.Stub(x => x.Please()).Throw(new RemoteApiException(expectedErrorMessage, new Response(HttpStatusCode.PaymentRequired, expectedErrorMessage), ErrorCode.PaymentFailed));

			var apiCardPurchaseRule = new ApiCardPurchaseRule(_stubPurchaseApi, _stubCatalogueApi);

			var cardPurchaseRequest = new CardPurchaseRequest { CardId = 1, Type = PurchaseType.release };
			var oAuthAccessToken = new OAuthAccessToken();

			var cardPurchaseStatus = apiCardPurchaseRule.FulfillPurchase(cardPurchaseRequest, oAuthAccessToken);

			Assert.That(cardPurchaseStatus.IsSuccess, Is.False);
			Assert.That(cardPurchaseStatus.Message, Is.EqualTo(expectedErrorMessage));
			Assert.That(cardPurchaseStatus.UpdatedLocker, Is.Empty);
		}

		[Test]
		public void Api_call_fired_with_correct_price()
		{
			_stubCatalogueApi.Stub(x => x.GetATrack(null, 0)).IgnoreArguments().Return(new Track()
			{
				Release = new Release
				{
					Id = 1
				}
			});

			_stubPurchaseApi.Stub(x => x.Please()).Return(new UserPurchaseItem());

			var apiCardPurchaseRule = new ApiCardPurchaseRule(_stubPurchaseApi, _stubCatalogueApi);
			var cardPurchaseRequest = new CardPurchaseRequest { CardId = 1, Price = 7.99m};
			var oAuthAccessToken = new OAuthAccessToken();

			apiCardPurchaseRule.FulfillPurchase(cardPurchaseRequest, oAuthAccessToken);
			_stubPurchaseApi.AssertWasCalled(x=>x.WithParameter("price", "7.99"));
		}

		[Test]
		public void Fails_if_invalid_cardid()
		{
			const string expectedErrorMessage = "Invalid card selected";
			
			var apiCardPurchaseRule = new ApiCardPurchaseRule(_stubPurchaseApi, _stubCatalogueApi);

			var cardPurchaseRequest = new CardPurchaseRequest { CardId = 0, Type = PurchaseType.release };
			var oAuthAccessToken = new OAuthAccessToken();

			var cardPurchaseStatus = apiCardPurchaseRule.FulfillPurchase(cardPurchaseRequest, oAuthAccessToken);

			Assert.That(cardPurchaseStatus.IsSuccess, Is.False);
			Assert.That(cardPurchaseStatus.Message, Is.EqualTo(expectedErrorMessage));
			Assert.That(cardPurchaseStatus.UpdatedLocker, Is.Empty);
		}


		private static IFluentApi<UserPurchaseItem> GetStubbed()
		{
			var generateStub = MockRepository.GenerateStub<IFluentApi<UserPurchaseItem>>();
			generateStub.Stub(x => x.ForUser(null, null)).IgnoreArguments().Return(generateStub);
			generateStub.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(generateStub);
			return generateStub;
		}
	}
}