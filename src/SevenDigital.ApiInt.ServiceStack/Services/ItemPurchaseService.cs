using System;
using System.Linq;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	[DefaultView("ItemPurchaseService")]
	public class ItemPurchaseService : Service
	{
		private readonly ICatalogue _catalogue;
		private readonly ILog _log = LogManager.GetLogger("ItemPurchaseService");

		public ItemPurchaseService(ICatalogue catalogue)
		{
			_catalogue = catalogue;
		}

		public object Get(ItemRequest request)
		{
			if (request.Id < 1)
				throw new ArgumentNullException("request", "You must specify an Id");

			if (string.IsNullOrEmpty(request.CountryCode))
				request.CountryCode = "GB";

			try
			{
				var internalResponse = GetInternalResponse(request);
				var response = BuildBrowseResponse(internalResponse);

				return new HttpResult(response);
			}
			catch (ApiException ex)
			{
				_log.Error(ex);
				Request.Items["View"] = "Error";
				throw new HttpError(request, HttpStatusCode.NotFound, "404", "Not found");
			}
		}

		private ReleaseAndTracks GetInternalResponse(ItemRequest request)
		{
			var releaseAndTracks = new ReleaseAndTracks{Type = request.Type};
			if (request.Type == PurchaseType.release)
			{
				releaseAndTracks.Release = _catalogue.GetARelease(request.CountryCode, request.Id);
				releaseAndTracks.Tracks = _catalogue.GetAReleaseTracks(request.CountryCode, request.Id);
			}
			else
			{
				var aTrack = _catalogue.GetATrack(request.CountryCode, request.Id);
				releaseAndTracks.Release = aTrack.Release;
				var releaseTracks = _catalogue.GetAReleaseTracks(request.CountryCode, aTrack.Release.Id);
				releaseAndTracks.Tracks = releaseTracks.Where(x => x.Id == request.Id).ToList();
			}
			return releaseAndTracks;
		}

		private BuyItNowResponse<ReleaseAndTracks> BuildBrowseResponse(ReleaseAndTracks internalResponse)
		{
			var authSession = this.GetSession();

			return new BuyItNowResponse<ReleaseAndTracks>
			{
				InternalResponse = internalResponse,
				Session = authSession,
			};
		}
	}
}