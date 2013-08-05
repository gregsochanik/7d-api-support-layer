using ServiceStack.ServiceInterface.Auth;
using SevenDigital.ApiSupportLayer.ServiceStack.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack
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