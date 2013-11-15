using ServiceStack.ServiceInterface;
using SevenDigital.ApiSupportLayer.ServiceStack.GeoLocation;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.Shops
{
	public class ShopRestrictionService : Service
	{
		private readonly IGeoLookup _geoLookup;

		public ShopRestrictionService(IGeoLookup geoLookup)
		{
			_geoLookup = geoLookup;
		}

		public ShopRestriction Get(ShopRestriction request)
		{
			request.IsRestricted = _geoLookup.IsRestricted(request.CountryCode, request.IpAddress);
			return request;
		}
	}
}