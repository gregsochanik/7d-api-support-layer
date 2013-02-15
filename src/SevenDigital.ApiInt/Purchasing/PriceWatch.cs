using System;
using SevenDigital.Api.Schema.Pricing;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Purchasing
{
	public class PriceWatch : IPriceWatch
	{
		private readonly ICatalogue _catalogue;

		public PriceWatch(ICatalogue catalogue)
		{
			_catalogue = catalogue;
		}

		public decimal GetItemPrice(ItemRequest request)
		{
			if (request.Type == PurchaseType.track)
				return GetTrackPrice(request.CountryCode, request.Id);

			return GetReleasePrice(request.CountryCode, request.Id);
		}

		public decimal GetReleasePrice(string countryCode, int id)
		{
			var aRelease = _catalogue.GetARelease(countryCode, id);
			return Convert.ToDecimal(aRelease.Price.Value);
		}

		public decimal GetTrackPrice(string countryCode, int id)
		{
			var aTrackWithPrice = _catalogue.GetATrackWithPrice(countryCode, id);
			if (aTrackWithPrice.Price.Status == PriceStatus.Free)
				return 0;
			return Convert.ToDecimal(aTrackWithPrice.Price.Value);
		}
	}
}