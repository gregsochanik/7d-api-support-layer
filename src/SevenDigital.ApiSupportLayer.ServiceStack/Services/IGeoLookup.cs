namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public interface IGeoLookup
	{
		bool IsRestricted(string countryCode, string ipAddress);
		string RestrictionMessage(string countryCode, string ipAddress);
	}
}