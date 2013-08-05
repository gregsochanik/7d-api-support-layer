namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class ShopRestriction
	{
		public string IpAddress { get; set; }
		public string CountryCode { get; set; }
		public bool IsRestricted { get; set; }
	}
}