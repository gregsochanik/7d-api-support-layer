using System;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Catalogue;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	[DefaultView("ItemPurchaseService")]
	public class ItemPurchaseService : Service
	{
		private readonly IProductCollater _productCollater;
		private readonly ILog _log = LogManager.GetLogger("ItemPurchaseService");

		public ItemPurchaseService(IProductCollater productCollater)
		{
			_productCollater = productCollater;
		}

		public object Get(ItemRequest request)
		{
			if (request.Id < 1)
				throw new ArgumentNullException("request", "You must specify an Id");

			try
			{
				var internalResponse = request.Type == PurchaseType.release
					                       ? _productCollater.UsingReleaseId(request.CountryCode, request.Id)
					                       : _productCollater.UsingTrackId(request.CountryCode, request.Id);

				//if (internalResponse.Type == PurchaseType.track && internalResponse.Tracks[0].Price.Status == PriceStatus.UnAvailable)
				//{
				//	// BUNDLE!!!!!
				//	request.Type = PurchaseType.release;
				//	request.Id = internalResponse.Release.Id;
				//	return Get(request);
				//}

				var response = ResponseHelper.BuildBuyItNowResponse(this.GetSession(), internalResponse);
				return new HttpResult(response);
			}
			catch (ApiException ex)
			{
				_log.Error(ex);
				Request.Items["View"] = "Error";
				throw new HttpError(request, HttpStatusCode.NotFound, "404", "Not found");
			}
		}
	}
}