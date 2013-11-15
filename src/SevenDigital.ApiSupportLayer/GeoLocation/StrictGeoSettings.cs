namespace SevenDigital.ApiSupportLayer.GeoLocation
{
	public class StrictGeoSettings : IGeoSettings
	{
		public bool IsTiedToIpAddress()
		{
			return true;
		}
	}
}