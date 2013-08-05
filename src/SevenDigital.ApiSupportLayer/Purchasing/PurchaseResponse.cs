using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Purchasing
{
	public class PurchaseResponse
	{
		public ItemRequest OriginalRequest { get; set; }
		public PurchaseStatus Status { get; set; }
		public PurchasedItem Item { get; set; } 
	}
}