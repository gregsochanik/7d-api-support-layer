using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.Basket;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.Premium.Basket;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Schema.User.Purchase;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Premium;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;
using SevenDigital.ApiInt.ServiceStack.Services;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class VoucherPurchaseServiceTests
	{
		private IFluentApi<CreateBasket> _createBasket;
		private IFluentApi<AddItemToBasket> _addItemToBasket;
		private IFluentApi<ApplyVoucherToBasket> _applyVoucher;
		private IFluentApi<UserPurchaseBasket> _userPurchaseBasket;
		private IPurchaseItemMapper _purchaseItemMapper;

		private ICatalogue _catalogue;
		private Guid _basketId;

		[SetUp]
		public void SetUp()
		{
			_createBasket = MockRepository.GenerateStub<IFluentApi<CreateBasket>>();
			_addItemToBasket = MockRepository.GenerateStub<IFluentApi<AddItemToBasket>>();
			_applyVoucher = MockRepository.GenerateStub<IFluentApi<ApplyVoucherToBasket>>();
			_userPurchaseBasket = MockRepository.GenerateStub<IFluentApi<UserPurchaseBasket>>();
			_purchaseItemMapper = MockRepository.GenerateStub<IPurchaseItemMapper>();

			_catalogue = MockRepository.GenerateStub<ICatalogue>();
			_basketId = Guid.NewGuid();

			_createBasket.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(_createBasket);
			_createBasket.Stub(x => x.Please()).Return(new CreateBasket
			{
				Id = _basketId.ToString()
			});

			_addItemToBasket.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(_addItemToBasket);
			_applyVoucher.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(_applyVoucher);
			_userPurchaseBasket.Stub(x => x.WithParameter(null, null)).IgnoreArguments().Return(_userPurchaseBasket);
			_userPurchaseBasket.Stub(x => x.ForUser(null, null)).IgnoreArguments().Return(_userPurchaseBasket);
			var lockerReleases = new List<LockerRelease>{new LockerRelease { Release = new Release(), LockerTracks = new List<LockerTrack>()}};
			_userPurchaseBasket.Stub(x => x.Please()).Return(new UserPurchaseBasket { LockerReleases = lockerReleases});

			_catalogue.Stub(x => x.GetATrack(null, 0)).IgnoreArguments().Return(new Track
			{
				Release = new Release
				{
					Id = 1
				}
			});
		}

		[Test]
		public void returns_cardvoucherresponse()
		{
			var voucherPurchaseRequest = new VoucherPurchaseRequest{VoucherCode="12345678"};
			var userVoucherService = new VoucherPurchaseService(_createBasket, _addItemToBasket, _applyVoucher, _userPurchaseBasket, _catalogue, _purchaseItemMapper)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			var voucherResponse = userVoucherService.Post(voucherPurchaseRequest);
			Assert.That(voucherResponse, Is.Not.Null);
			Assert.That(voucherResponse.VoucherCode, Is.EqualTo(voucherPurchaseRequest.VoucherCode));
		}

		[Test]
		public void creates_a_basket()
		{
			var cardVoucherRequest = new VoucherPurchaseRequest { VoucherCode = "12345678" };
			var userVoucherService = new VoucherPurchaseService(_createBasket, _addItemToBasket, _applyVoucher, _userPurchaseBasket, _catalogue, _purchaseItemMapper)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			userVoucherService.Post(cardVoucherRequest);

			_createBasket.AssertWasCalled(x=>x.Please());
		}

		[Test]
		public void adds_release_to_that_basket_if_a_releaseRequest()
		{
			var cardVoucherRequest = new VoucherPurchaseRequest { VoucherCode = "12345678", Type = PurchaseType.release, Id = 1234};
			var userVoucherService = new VoucherPurchaseService(_createBasket, _addItemToBasket, _applyVoucher, _userPurchaseBasket, _catalogue, _purchaseItemMapper)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			userVoucherService.Post(cardVoucherRequest);
			_addItemToBasket.AssertWasCalled(x => x.ForReleaseId(1234));
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

			var cardVoucherRequest = new VoucherPurchaseRequest { VoucherCode = "12345678", Type = PurchaseType.track, Id = expectedTrackId };
			var userVoucherService = new VoucherPurchaseService(_createBasket, _addItemToBasket, _applyVoucher, _userPurchaseBasket, catalogue, _purchaseItemMapper)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			userVoucherService.Post(cardVoucherRequest);
			_addItemToBasket.AssertWasCalled(x => x.ForTrackId(expectedTrackId));
			_addItemToBasket.AssertWasCalled(x => x.ForReleaseId(expectedReleaseId));
		}

		[Test]
		public void applys_voucher_to_that_basket()
		{
			const int expectedReleaseId = 12345;
			const int expectedTrackId = 1234;
			const string expectedVoucherCode = "12345678";

			var catalogue = MockRepository.GenerateStub<ICatalogue>();

			catalogue.Stub(x => x.GetATrack(null, 0)).IgnoreArguments().Return(new Track
			{
				Release = new Release
				{
					Id = expectedReleaseId
				}
			});

			var cardVoucherRequest = new VoucherPurchaseRequest { VoucherCode = expectedVoucherCode, Type = PurchaseType.track, Id = expectedTrackId };
			var userVoucherService = new VoucherPurchaseService(_createBasket, _addItemToBasket, _applyVoucher, _userPurchaseBasket, catalogue, _purchaseItemMapper)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};
			userVoucherService.Post(cardVoucherRequest);

			_applyVoucher.AssertWasCalled(x => x.UseVoucherCode(expectedVoucherCode));
		}
	}
}