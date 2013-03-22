using System;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	[DefaultView("ItemPurchaseService")]
	public class SpecificTrackPurchaseService : Service
	{
		private readonly IProductCollater _productCollater;

		public SpecificTrackPurchaseService(IProductCollater productCollater)
		{
			_productCollater = productCollater;
		}

		public object Get(SpecificTrackRequest request)
		{
			if (request.ReleaseId < 1)
				throw new ArgumentNullException("request", "You must specify a ReleaseId");

			if (request.Id < 1)
				throw new ArgumentNullException("request", "You must specify an Id");

			var releaseAndTracks = _productCollater.UsingReleaseAndTrackId(request.CountryCode, request.ReleaseId, request.Id);

			var response = ResponseHelper.BuildBuyItNowResponse(this.GetSession(), releaseAndTracks);
			return new HttpResult(response);
		}
	}
}