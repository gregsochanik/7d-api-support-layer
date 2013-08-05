namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class StrictGeoSettings : IGeoSettings
	{
		public bool IsTiedToIpAddress()
		{
			return true;
		}
	}
}