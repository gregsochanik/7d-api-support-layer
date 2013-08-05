using ServiceStack.ServiceInterface;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Model
{
	[Authenticate]
	public class VoucherPurchaseRequest : ItemRequest
	{
		public string VoucherCode { get; set; }
	}
}