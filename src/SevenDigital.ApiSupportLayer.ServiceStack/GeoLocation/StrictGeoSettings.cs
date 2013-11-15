namespace SevenDigital.ApiSupportLayer.ServiceStack.GeoLocation
{
	public class StrictGeoSettings : IGeoSettings
	{
		public bool IsTiedToIpAddress()
		{
			return true;
		}
	}
}