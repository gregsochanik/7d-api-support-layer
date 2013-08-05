using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Model
{
	public class LockerCheckResponse
	{
		public LockerCheckRequest OriginalRequest { get; set; }
		public PurchasedItem Item { get; set; }
	}
}