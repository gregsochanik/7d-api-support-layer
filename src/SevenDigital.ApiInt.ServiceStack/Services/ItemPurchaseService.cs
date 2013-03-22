using System;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.Pricing;
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

		public HttpResult Get(ItemRequest request)
		{
			if (request.Id < 1)
				throw new ArgumentNullException("request", "You must specify an Id");

			try
			{
				var collatedReleaseAndTracks = request.HasReleaseId() 
					? BasedOnSpecificRelease(request.CountryCode, request.ReleaseId.Value, request.Id) 
					: BasedOnPurchaseType(request);

				if (collatedReleaseAndTracks.IsABundleTrack())
				{
					return RedirectToReleaseBundle(request, collatedReleaseAndTracks.Release.Id);
				}

				return new HttpResult(ResponseHelper.BuildBuyItNowResponse(this.GetSession(), collatedReleaseAndTracks));
			}
			catch (ApiException ex)
			{
				_log.Error(ex);
				Request.Items["View"] = "Error";
				throw new HttpError(request, HttpStatusCode.NotFound, "404", "Not found");
			}
		}

		private ReleaseAndTracks BasedOnPurchaseType(ItemRequest request)
		{
			return request.Type == PurchaseType.release
				       ? _productCollater.UsingReleaseId(request.CountryCode, request.Id)
				       : _productCollater.UsingTrackId(request.CountryCode, request.Id);
		}

		private ReleaseAndTracks BasedOnSpecificRelease(string countryCode, int releaseId, int trackId)
		{
			return _productCollater.UsingReleaseAndTrackId(countryCode, releaseId, trackId);
		}

		private HttpResult RedirectToReleaseBundle(ItemRequest request, int releaseId)
		{
			request.Type = PurchaseType.release;
			request.Id = releaseId;
			return Get(request);
		}
	}

	public static class ReleaseAndTracksExtension
	{
		public static bool IsABundleTrack(this ReleaseAndTracks collatedReleaseAndTracks)
		{
			if (collatedReleaseAndTracks == null || collatedReleaseAndTracks.Tracks == null)
				return false;

			if (collatedReleaseAndTracks.Tracks.Count < 1)
				return false;

			if (collatedReleaseAndTracks.Tracks[0].Price == null)
				return false;

			return collatedReleaseAndTracks.Type == PurchaseType.track 
				&& collatedReleaseAndTracks.Tracks[0].Price.Status == PriceStatus.UnAvailable;
		}
	}

	public static class ItemRequestExtension
	{
		public static bool HasReleaseId(this ItemRequest request)
		{
			return request.ReleaseId.HasValue && request.ReleaseId > 0 && request.Type == PurchaseType.track;
		}
	}
}