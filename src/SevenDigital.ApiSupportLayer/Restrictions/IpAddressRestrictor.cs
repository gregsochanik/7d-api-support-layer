using System.Collections.Generic;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiSupportLayer.GeoLocation;

namespace SevenDigital.ApiSupportLayer.Restrictions
{
	public class IpAddressRestrictor : IRestrictor
	{
		private readonly IGeoLookup _geoLookup;
		private readonly IGeoSettings _geoSettings;

		public IpAddressRestrictor(IGeoLookup geoLookup, IGeoSettings geoSettings)
		{
			_geoLookup = geoLookup;
			_geoSettings = geoSettings;
		}

		public Restriction AssertRestriction(KeyValuePair<string, string> restriction)
		{
			var countryCode = restriction.Key;
			var ipAddress = restriction.Value;

			try
			{
				if (_geoSettings.IsTiedToIpAddress() && _geoLookup.IsRestricted(countryCode, ipAddress))
				{
					return new Restriction(RestrictionType.TerritoryRestriction, _geoLookup.RestrictionMessage(countryCode, ipAddress));
				}
			}
			catch (InputParameterException iex)
			{
				return new Restriction(RestrictionType.TerritoryRestrictionInvalidIpAddress, iex.Message);
			}

			return new Restriction(RestrictionType.NoRestriction, "");
		}
	}
}