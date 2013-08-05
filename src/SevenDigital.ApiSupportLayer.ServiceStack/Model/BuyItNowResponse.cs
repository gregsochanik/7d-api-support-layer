using ServiceStack.ServiceInterface.Auth;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Model
{
	public class BuyItNowResponse<T>
	{
		public IAuthSession Session { get; set; }
		public T InternalResponse { get; set; }
		public bool IsPurchased { get; set; }
		public PurchaseType Type { get; set; }
	}
}