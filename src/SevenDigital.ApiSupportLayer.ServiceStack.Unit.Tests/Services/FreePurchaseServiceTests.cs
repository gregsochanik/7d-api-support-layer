using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.Common.Web;
using SevenDigital.Api.Schema.Pricing;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.ApiSupportLayer.Basket;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.Mapping;
using SevenDigital.ApiSupportLayer.Model;
using SevenDigital.ApiSupportLayer.ServiceStack.Model;
using SevenDigital.ApiSupportLayer.ServiceStack.Services;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class FreePurchaseServiceTests
	{
		private const int PRICED_TRACK_ID = 1234;
		private IPurchaseItemMapper _mapper;
		private static readonly Track _pricedTrack = new Track { Id = PRICED_TRACK_ID, Price = new Price() };
		private static readonly Release _pricedRelease = new Release { Id = PRICED_TRACK_ID, Price = new Price() };
		private PurchasedItem _expectedPurchasedItem;
		private IBasketHandler _basketHandler;

		[SetUp]
		public void SetUp()
		{
			_mapper = MockRepository.GenerateStub<IPurchaseItemMapper>();

			_basketHandler = MockRepository.GenerateStub<IBasketHandler>();

			_expectedPurchasedItem = new PurchasedItem();
			_mapper.Stub(x => x.Map(null, null)).IgnoreArguments().Return(_expectedPurchasedItem);

		}

		[Test]
		public void Throws_error_if_track_is_not_free()
		{
			var catalogue = MockRepository.GenerateStub<ICatalogue>();
			catalogue.Stub(x => x.GetATrackWithPrice(null, 0)).IgnoreArguments().Return(_pricedTrack);

			var freePurchaseService = new FreePurchaseService(_mapper, _basketHandler, catalogue)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			var freePurchaseRequest = new FreePurchaseRequest { CountryCode = "GB", Id = PRICED_TRACK_ID, Type = PurchaseType.track };

			var httpError = Assert.Throws<HttpError>(() => freePurchaseService.Post(freePurchaseRequest));
			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
			Assert.That(httpError.Message, Is.EqualTo(string.Format("This track is not free! {0}", _pricedTrack.Price.Status)));
			Assert.That(httpError.ErrorCode, Is.EqualTo("TrackNotFree"));
		}

		[Test]
		public void Throws_error_if_release_is_not_free()
		{
			var catalogue = MockRepository.GenerateStub<ICatalogue>();
			catalogue.Stub(x => x.GetARelease(null, 0)).IgnoreArguments().Return(_pricedRelease);

			var freePurchaseService = new FreePurchaseService(_mapper, _basketHandler, catalogue)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			var freePurchaseRequest = new FreePurchaseRequest { CountryCode = "GB", Id = PRICED_TRACK_ID, Type = PurchaseType.release };

			var httpError = Assert.Throws<HttpError>(() => freePurchaseService.Post(freePurchaseRequest));
			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
			Assert.That(httpError.Message, Is.EqualTo(string.Format("This release is not free! {0}", _pricedTrack.Price.Status)));
			Assert.That(httpError.ErrorCode, Is.EqualTo("ReleaseNotFree"));
		}
	}
}