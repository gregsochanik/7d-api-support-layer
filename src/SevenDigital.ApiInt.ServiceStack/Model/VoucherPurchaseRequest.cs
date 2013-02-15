using ServiceStack.ServiceInterface;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Model
{
	[Authenticate]
	public class VoucherPurchaseRequest : ItemRequest
	{
		public string VoucherCode { get; set; }
	}
}