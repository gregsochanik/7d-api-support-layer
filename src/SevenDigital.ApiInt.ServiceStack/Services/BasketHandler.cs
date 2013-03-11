using System;
using SevenDigital.Api.Schema.Basket;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class BasketHandler : IBasketHandler
	{
		private readonly IFluentApi<CreateBasket> _createBasket;
		private readonly IFluentApi<AddItemToBasket> _addItemToBasket;
		private readonly ICatalogue _catalogue;

		public BasketHandler(IFluentApi<CreateBasket> createBasket, IFluentApi<AddItemToBasket> addItemToBasket, ICatalogue catalogue)
		{
			_createBasket = createBasket;
			_addItemToBasket = addItemToBasket;
			_catalogue = catalogue;
		}

		public Guid Create(ItemRequest request)
		{
			var createBasket = _createBasket.WithParameter("country", request.CountryCode).Please();
			return new Guid(createBasket.Id);
		}

		public Basket AddItem(Guid basketId, ItemRequest request)
		{
			_addItemToBasket.UseBasketId(basketId);
			AdjustApiCallBasedOnPurchaseType(_addItemToBasket, request);
			return _addItemToBasket.WithParameter("country", request.CountryCode)
			                       .WithParameter("affiliatePartner", request.PartnerId.ToString())
			                       .Please();
		}

		private void AdjustApiCallBasedOnPurchaseType(IFluentApi<AddItemToBasket> api, ItemRequest request)
		{
			if (request.Type == PurchaseType.release)
			{
				api.ForReleaseId(request.Id);
			}
			else
			{
				var track = _catalogue.GetATrack(request.CountryCode, request.Id);
				var releaseId = track.Release.Id;
				api.ForReleaseId(releaseId).ForTrackId(request.Id);
			}
		}
	}
}