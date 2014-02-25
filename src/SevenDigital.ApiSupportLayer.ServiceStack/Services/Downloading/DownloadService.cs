using System.Collections.Generic;
using System.Linq;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.ApiSupportLayer.Authentication;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.Locker;
using SevenDigital.ApiSupportLayer.MediaDelivery;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.Downloading
{
	public class DownloadErrorRequest : ItemRequest
	{}

	public class DownloadErrorService : Service
	{
		public DownloadErrorRequest Get(DownloadErrorRequest errorDetails)
		{
			return errorDetails;
		}
	}

	public class DownloadRequest : ItemRequest
	{
		public string ErrorUrl { get; set; }
	}

	[Authenticate]
	public class DownloadBestFormatService : Service
	{
		private readonly IOAuthSigner _urlSigner;
		private readonly ICatalogue _catalogue;
		private readonly ILockerBrowser _lockerBrowser;
		private readonly ILog _logger = LogManager.GetLogger("DownloadBestFormatService");

		public DownloadBestFormatService(IOAuthSigner urlSigner, ICatalogue catalogue, ILockerBrowser lockerBrowser)
		{
			_urlSigner = urlSigner;
			_catalogue = catalogue;
			_lockerBrowser = lockerBrowser;
		}

		public HttpResult Get(DownloadRequest request)
		{
			var oAuthAccessToken = this.TryGetOAuthAccessToken();
			
			// check users locker for item
			var lockerResponse = _lockerBrowser.GetLockerItem(oAuthAccessToken, request);
			if (lockerResponse.TotalItems < 1)
			{
				_logger.WarnFormat("Illegal download attempt {0}", request.Id);
				throw new HttpError(HttpStatusCode.Forbidden, "NotOwned", "You do not own this " + request.Type);
			}

			var url = request.Type == PurchaseType.release
				? DownloadSettings.DOWNLOAD_RELEASE_URL
				: DownloadSettings.DOWNLOAD_TRACK_URL;

			var parameters = BuildDownloadParameters(request, lockerResponse);
			var signGetUrl = _urlSigner.SignGetRequest(url, oAuthAccessToken, parameters);
			return new HttpResult
			{
				Headers = { { "Cache-control", "no-cache" } },
				Location = signGetUrl,
				StatusCode = HttpStatusCode.Redirect
			};
		}

		private Dictionary<string,string> BuildDownloadParameters(ItemRequest request, LockerResponse locker)
		{
			_logger.InfoFormat("DownloadUrl requested for {0} {1}", request.Id, request.Type);

			if (request.Type == PurchaseType.release)
			{
				return new Dictionary<string, string>
				{
					{"releaseId", request.Id.ToString()},
					{"country", request.CountryCode},
				};
			}

			var track = locker.LockerReleases[0].LockerTracks.First(x => x.Track.Id == request.Id);
			if (track == null)
			{
				_logger.ErrorFormat("Could not find track id {0} in users locker", request.Id);
				throw new HttpError(HttpStatusCode.Forbidden, "NotOwned", "You do not own this " + request.Type);
			}
			var aTrack = _catalogue.GetATrack(request.CountryCode, request.Id);
			return new Dictionary<string, string>
			{
				{"releaseId", aTrack.Release.Id.ToString()},
				{"trackid", aTrack.Id.ToString()},
				{"formatid", track.DownloadUrls[0].Format.Id.ToString()},
				{"country", request.CountryCode},
			};
		}
	}
}
