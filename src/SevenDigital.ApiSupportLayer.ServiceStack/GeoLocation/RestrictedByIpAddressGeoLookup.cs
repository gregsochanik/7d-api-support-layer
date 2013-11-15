using SevenDigital.Api.Schema.Territories;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiSupportLayer.ServiceStack.Services.Shops;

namespace SevenDigital.ApiSupportLayer.ServiceStack.GeoLocation
{
	public class GeoLookup : IGeoLookup
	{
		private readonly IFluentApi<GeoIpLookup> _ipLookupApi;
		private readonly ShopUrlService _shopUrlService;

		public GeoLookup(IFluentApi<GeoIpLookup> ipLookupApi, ShopUrlService shopUrlService)
		{
			_ipLookupApi = ipLookupApi;
			_shopUrlService = shopUrlService;
		}

		public bool IsRestricted(string countryCode, string ipAddress)
		{
			var geoIpLookup = _ipLookupApi.WithIpAddress(ipAddress).Please();
			return countryCode != geoIpLookup.CountryCode;
		}

		public string RestrictionMessage(string countryCode, string ipAddress)
		{
			var requestedShopDetails = _shopUrlService.Get(new ShopUrl
			{
				CountryCode = countryCode
			});
			var requestedShopUrl = requestedShopDetails.Headers["Location"];
			var requestedShopName = requestedShopDetails.Response;

			var geoIpLookup = _ipLookupApi.WithIpAddress(ipAddress).Please();
			var localShopDetails = _shopUrlService.Get(new ShopUrl
			{
				CountryCode = geoIpLookup.CountryCode
			});

			var localShopUrl = localShopDetails.Headers["Location"];

			return "Sorry, but in accordance with our contractual obligations with the labels, you can only purchase from " + requestedShopUrl + " if you live in " + requestedShopName + ".\r\nPlease visit your local store at " + localShopUrl + " to find an alternative version. If you’ve tried to access " + requestedShopUrl + " from inside " + requestedShopName + ", please get in touch with our Customer Support Team who will resolve the problem for you.\r\nThanks for your understanding!";
		}
	}
}