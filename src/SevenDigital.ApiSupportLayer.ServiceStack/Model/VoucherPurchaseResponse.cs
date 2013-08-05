using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Model
{
	public class VoucherPurchaseResponse
	{
		public VoucherPurchaseRequest OriginalRequest { get; set; }
		public PurchaseStatus Status { get; set; }
		public PurchasedItem Item { get; set; }
		public string VoucherCode { get; set; }
	}
}