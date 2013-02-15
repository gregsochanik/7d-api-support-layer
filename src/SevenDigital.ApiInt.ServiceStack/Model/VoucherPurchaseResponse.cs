using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Model
{
	public class VoucherPurchaseResponse
	{
		public VoucherPurchaseRequest OriginalRequest { get; set; }
		public PurchaseStatus Status { get; set; }
		public PurchasedItem Item { get; set; }
		public string VoucherCode { get; set; }
	}
}