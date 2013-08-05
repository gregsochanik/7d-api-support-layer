using System.Collections.Generic;
using ServiceStack.ServiceInterface.Auth;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.Purchasing;
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