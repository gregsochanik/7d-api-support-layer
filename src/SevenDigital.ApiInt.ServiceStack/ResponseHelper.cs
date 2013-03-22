using ServiceStack.ServiceInterface.Auth;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack
{
	public static class ResponseHelper
	{
		public static BuyItNowResponse<T> BuildBuyItNowResponse<T>(IAuthSession authSession, T internalResponse)
		{
			return new BuyItNowResponse<T>
			{
				InternalResponse = internalResponse,
				Session = authSession,
			};
		}
	}
}