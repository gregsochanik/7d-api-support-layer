using System;
using System.Linq;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public interface IProductCollater
	{
		ReleaseAndTracks UsingReleaseId(string countryCode, int releaseId);
		ReleaseAndTracks UsingTrackId(string countryCode, int trackId);
		ReleaseAndTracks UsingReleaseAndTrackId(string countryCode, int releaseId, int trackId);
	}

	public class ProductCollater : IProductCollater
	{
		private readonly ICatalogue _catalogue;

		public ProductCollater(ICatalogue catalogue)
		{
			_catalogue = catalogue;
		}

		public ReleaseAndTracks UsingReleaseId(string countryCode, int releaseId)
		{
			return new ReleaseAndTracks
			{
				Type = PurchaseType.release,
				Release = _catalogue.GetARelease(countryCode, releaseId),
				Tracks = _catalogue.GetAReleaseTracks(countryCode, releaseId)
			};
		}

		public ReleaseAndTracks UsingTrackId(string countryCode, int trackId)
		{
			var aTrack = _catalogue.GetATrack(countryCode, trackId);
			var releaseTracks = _catalogue.GetAReleaseTracks(countryCode, aTrack.Release.Id);
			var aRelease = _catalogue.GetARelease(countryCode, aTrack.Release.Id);

			return new ReleaseAndTracks
			{
				Type = PurchaseType.track,
				Release = aRelease,
				Tracks = releaseTracks.Where(x => x.Id == trackId).ToList()
			};
		}

		public ReleaseAndTracks UsingReleaseAndTrackId(string countryCode, int releaseId, int trackId)
		{
			var releaseTracks = _catalogue.GetAReleaseTracks(countryCode, releaseId);
			var aRelease = _catalogue.GetARelease(countryCode, releaseId);

			return new ReleaseAndTracks
			{
				Type = PurchaseType.track,
				Release = aRelease,
				Tracks = releaseTracks.Where(x => x.Id == trackId).ToList()
			};
		}
	}

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