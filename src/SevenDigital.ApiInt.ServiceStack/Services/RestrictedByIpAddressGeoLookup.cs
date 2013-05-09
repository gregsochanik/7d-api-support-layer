using System.Web;
using SevenDigital.Api.Schema.Territories;
using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiInt.ServiceStack.Services
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

			return HttpUtility.HtmlEncode("<p>Sorry, but in accordance with our contractual obligations with the labels, you can only purchase from " + requestedShopUrl + " if you live in " + requestedShopName + ".</p><p>Please visit your local store at " + localShopUrl + " to find an alternative version. If you’ve tried to access " + requestedShopUrl + " from inside " + requestedShopName + ", please get in touch with our Customer Support Team who will resolve the problem for you.</p><p>Thanks for your understanding!</p>");
		}
	}
}