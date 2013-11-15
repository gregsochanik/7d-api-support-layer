namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.Shops
{
	public class ShopRestriction
	{
		public string IpAddress { get; set; }
		public string CountryCode { get; set; }
		public bool IsRestricted { get; set; }
	}
}