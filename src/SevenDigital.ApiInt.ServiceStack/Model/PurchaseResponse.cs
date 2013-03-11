using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Model
{
	public class PurchaseResponse
	{
		public ItemRequest OriginalRequest { get; set; }
		public PurchaseStatus Status { get; set; }
		public PurchasedItem Item { get; set; } 
	}
}