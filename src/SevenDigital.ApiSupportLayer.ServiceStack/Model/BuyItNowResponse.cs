using ServiceStack.ServiceInterface.Auth;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Model
{
	public class BuyItNowResponse<T>
	{
		public IAuthSession Session { get; set; }
		public T InternalResponse { get; set; }
		public bool IsPurchased { get; set; }
		public PurchaseType Type { get; set; }
	}
}