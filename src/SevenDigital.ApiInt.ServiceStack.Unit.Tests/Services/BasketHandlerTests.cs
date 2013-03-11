using System;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.Basket;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Services;
using SevenDigital.ApiInt.TestData;
using SevenDigital.ApiInt.TestData.StubApiWrapper;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class BasketHandlerTests
	{
		private IFluentApi<CreateBasket> _createBasketApi;
		private IFluentApi<AddItemToBasket> _addToBasketApi;
		private ICatalogue _catalogue;
		private Guid _expectedBasketGuid = Guid.NewGuid();
		private IFluentApi<UserPurchaseBasket> _userPurchaseBasket;

		[SetUp]
		public void SetUp()
		{
			_createBasketApi = MockRepository.GenerateStub<IFluentApi<CreateBasket>>();

			_addToBasketApi = MockRepository.GenerateStub<IFluentApi<AddItemToBasket>>();
			_addToBasketApi = ApiWrapper.StubbedTypedFluentApi(new AddItemToBasket
			{
				Id = _expectedBasketGuid.ToString()
			});

			_userPurchaseBasket = ApiWrapper.StubbedTypedFluentApiWthUser(new UserPurchaseBasket());

			_catalogue = MockRepository.GenerateStub<ICatalogue>();
			_catalogue.Stub(x => x.GetATrack(null, 0)).IgnoreArguments().Return(TestData.TestTrack.SunItRises);


		}

		[Test]
		public void CreateBasket_fires_api_call_and_returns_generated_id()
		{
			var expectedBasketId = _expectedBasketGuid.ToString();
			var createBasket = ApiWrapper.StubbedTypedFluentApi(new CreateBasket
			{
				Id = expectedBasketId
			});

			var basketHandler = new BasketHandler(createBasket, _addToBasketApi, _userPurchaseBasket, _catalogue);
			var actualBasketId = basketHandler.Create(new ItemRequest
			{
				CountryCode = "GB"
			});

			createBasket.AssertWasCalled(x => x.WithParameter("country", "GB"));
			createBasket.AssertWasCalled(x => x.Please());
			Assert.That(actualBasketId, Is.EqualTo(_expectedBasketGuid));
		}

		[Test]
		public void AddBasket_fires_api_call_and_returns_basket()
		{
			const int expectedPartnerId = 1;
			const string expectedCountryCode = "GB";

			var basketHandler = new BasketHandler(_createBasketApi, _addToBasketApi, _userPurchaseBasket, _catalogue);
			var itemRequest = new ItemRequest
			{
				CountryCode = expectedCountryCode,
				PartnerId = expectedPartnerId
			};
			var addItem = basketHandler.AddItem(_expectedBasketGuid, itemRequest);

			_addToBasketApi.AssertWasCalled(x => x.WithParameter("country", expectedCountryCode));
			_addToBasketApi.AssertWasCalled(x => x.WithParameter("affiliatePartner", expectedPartnerId.ToString()));
			_addToBasketApi.AssertWasCalled(x => x.Please());

			Assert.That(addItem.Id, Is.EqualTo(_expectedBasketGuid.ToString()));
		}


		[Test]
		public void adds_release_to_that_basket_if_a_releaseRequest()
		{
			var itemRequest = new ItemRequest
			{
				Type = PurchaseType.release,
				Id = 1234
			};
			var basketHandler = new BasketHandler(_createBasketApi, _addToBasketApi, _userPurchaseBasket, _catalogue);

			basketHandler.AddItem(_expectedBasketGuid, itemRequest);
			_addToBasketApi.AssertWasCalled(x => x.ForReleaseId(1234));
		}

		[Test]
		public void adds_track_to_that_basket_if_a_trackRequest()
		{
			const int expectedReleaseId = 12345;
			const int expectedTrackId = 1234;

			var catalogue = MockRepository.GenerateStub<ICatalogue>();

			catalogue.Stub(x => x.GetATrack(null, 0)).IgnoreArguments().Return(new Track
			{
				Release = new Release
				{
					Id = expectedReleaseId
				}
			});

			var itemRequest = new ItemRequest
			{
				Type = PurchaseType.track,
				Id = expectedTrackId
			};
			var basketHandler = new BasketHandler(_createBasketApi, _addToBasketApi, _userPurchaseBasket, catalogue);
			basketHandler.AddItem(_expectedBasketGuid, itemRequest);
			_addToBasketApi.AssertWasCalled(x => x.ForTrackId(expectedTrackId));
			_addToBasketApi.AssertWasCalled(x => x.ForReleaseId(expectedReleaseId));
		}

		[Test]
		public void Purchase_calls_api_with_correct_parameters()
		{
			var basketHandler = new BasketHandler(_createBasketApi, _addToBasketApi, _userPurchaseBasket, _catalogue);

			basketHandler.Purchase(_expectedBasketGuid, "GB", FakeUserData.FakeAccessToken);

			_userPurchaseBasket.AssertWasCalled(x => x.WithParameter("country", "GB"));
			_userPurchaseBasket.AssertWasCalled(
				x => x.ForUser(FakeUserData.FakeAccessToken.Token, FakeUserData.FakeAccessToken.Secret));
		}
	}
}