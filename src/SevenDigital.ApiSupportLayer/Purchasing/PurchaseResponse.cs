using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Purchasing
{
	public class PurchaseResponse
	{
		public ItemRequest OriginalRequest { get; set; }
		public PurchaseStatus Status { get; set; }
		public PurchasedItem Item { get; set; } 
	}
}