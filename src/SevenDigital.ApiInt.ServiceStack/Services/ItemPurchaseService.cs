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
	public class ItemPurchaseService : Service
	{
		private readonly IProductCollater _productCollater;
		private readonly IGeoLookup _geoLookup;
		private readonly ILog _log = LogManager.GetLogger("ItemPurchaseService");

		public ItemPurchaseService(IProductCollater productCollater, IGeoLookup geoLookup)
		{
			_productCollater = productCollater;
			_geoLookup = geoLookup;
		}

		public HttpResult Get(ItemRequest request)
		{
			if (_geoLookup.IsRestricted(request.CountryCode, Request.RemoteIp))
			{
				throw new HttpError(HttpStatusCode.Forbidden, "TerritoryRestriction", _geoLookup.RestrictionMessage(request.CountryCode, Request.RemoteIp));
			}

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
			catch (InvalidResourceException ex)
			{
				_log.Warn(ex);
				throw new HttpError(request, HttpStatusCode.NotFound, ex.ErrorCode.ToString(), "Not found");
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
}