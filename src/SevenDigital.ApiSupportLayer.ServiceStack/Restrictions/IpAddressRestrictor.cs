using System.Collections.Generic;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiSupportLayer.ServiceStack.GeoLocation;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Restrictions
{
	public class IpAddressRestrictor : IRestrictor
	{
		private readonly IGeoLookup _geoLookup;
		private readonly IGeoSettings _geoSettings;
		private readonly ILog _log = LogManager.GetLogger("IpAddressRestrictor");

		public IpAddressRestrictor(IGeoLookup geoLookup, IGeoSettings geoSettings)
		{
			_geoLookup = geoLookup;
			_geoSettings = geoSettings;
		}

		public void AssertRestriction(KeyValuePair<string,string> restriction)
		{
			var countryCode = restriction.Key;
			var ipAddress = restriction.Value;

			try
			{
				if (_geoSettings.IsTiedToIpAddress() && _geoLookup.IsRestricted(countryCode, ipAddress))
				{
					_log.WarnFormat("TerritoryRestriction: {0} {1}", ipAddress, countryCode);
					throw new HttpError(HttpStatusCode.Forbidden, "TerritoryRestriction", _geoLookup.RestrictionMessage(countryCode, ipAddress));
				}
			}
			catch (InputParameterException iex)
			{
				_log.ErrorFormat("TerritoryRestrictionInvalidIpAddress: {0} {1}", ipAddress, countryCode);
				throw new HttpError(HttpStatusCode.Forbidden, "TerritoryRestrictionInvalidIpAddress", iex.Message);
			}
		}
	}
}