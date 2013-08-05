using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Model
{
	public class LockerCheckResponse
	{
		public LockerCheckRequest OriginalRequest { get; set; }
		public PurchasedItem Item { get; set; }
	}
}