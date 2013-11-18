namespace SevenDigital.ApiSupportLayer.GeoLocation
{
	public interface IGeoLookup
	{
		bool IsRestricted(string countryCode, string ipAddress);
		string RestrictionMessage(string countryCode, string ipAddress);
	}
}