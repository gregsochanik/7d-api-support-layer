using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.Territories;
using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class ShopRestrictionService : Service
	{
		private readonly IFluentApi<GeoIpLookup> _ipLookupApi;

		public ShopRestrictionService(IFluentApi<GeoIpLookup> ipLookupApi)
		{
			_ipLookupApi = ipLookupApi;
		}

		public ShopRestriction Get(ShopRestriction request)
		{
			var geoIpLookup = _ipLookupApi.WithIpAddress(request.IpAddress).Please();
			request.IsRestricted = request.CountryCode != geoIpLookup.CountryCode;
			return request;
		}
	}
}