using System.Linq;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.Locker;
using SevenDigital.ApiSupportLayer.MediaDelivery;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services
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
		private readonly IUrlSigner _urlSigner;
		private readonly IOAuthCredentials _configAuthCredentials;
		private readonly ICatalogue _catalogue;
		private readonly ILockerBrowser _lockerBrowser;
		private readonly ILog _logger = LogManager.GetLogger("DownloadBestFormatService");

		public DownloadBestFormatService(IUrlSigner urlSigner, IOAuthCredentials configAuthCredentials, ICatalogue catalogue, ILockerBrowser lockerBrowser)
		{
			_urlSigner = urlSigner;
			_configAuthCredentials = configAuthCredentials;
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

			var url = BuildDownloadUrl(request, lockerResponse);

			string signGetUrl = _urlSigner.SignGetUrl(url, oAuthAccessToken.Token, oAuthAccessToken.Secret, _configAuthCredentials);
			return new HttpResult
			{
				Headers = { { "Cache-control", "no-cache" } },
				Location = signGetUrl,
				StatusCode = HttpStatusCode.Redirect
			};
		}

		private string BuildDownloadUrl(DownloadRequest request, LockerResponse locker)
		{
			string downloadUrl;
			if (request.Type == PurchaseType.release)
			{
				downloadUrl = BuildReleaseUrl(request);
			}
			else
			{
				var track = locker.LockerReleases[0].LockerTracks.First(x => x.Track.Id == request.Id);
				if (track == null)
				{
					_logger.ErrorFormat("Could not find track id {0} in users locker", request.Id);
					throw new HttpError(HttpStatusCode.Forbidden, "NotOwned", "You do not own this " + request.Type);
				}
				downloadUrl = BuildTrackUrl(request, track.DownloadUrls[0].Format.Id);
			}

			_logger.InfoFormat("DownloadUrl requested for {0} {1}", request.Id, request.Type);
			_logger.Debug(downloadUrl);
			return downloadUrl;
		}

		private static string BuildReleaseUrl(ItemRequest request)
		{
			return string.Format("{0}?releaseid={1}&country={2}",
			                     DownloadSettings.DOWNLOAD_RELEASE_URL,
			                     request.Id,
			                     request.CountryCode);
		}

		private string BuildTrackUrl(ItemRequest request, int formatId)
		{
			var aTrack = _catalogue.GetATrack(request.CountryCode, request.Id);
			return string.Format("{0}?releaseid={1}&trackid={2}&country={3}&formatId={4}",
										DownloadSettings.DOWNLOAD_TRACK_URL,
										aTrack.Release.Id,
										aTrack.Id,
										request.CountryCode, 
										formatId);
		}
	}
}
