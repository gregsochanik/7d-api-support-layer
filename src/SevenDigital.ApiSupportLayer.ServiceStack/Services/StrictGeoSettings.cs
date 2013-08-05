namespace SevenDigital.ApiSupportLayer.ServiceStack.Services
{
	public class StrictGeoSettings : IGeoSettings
	{
		public bool IsTiedToIpAddress()
		{
			return true;
		}
	}
}