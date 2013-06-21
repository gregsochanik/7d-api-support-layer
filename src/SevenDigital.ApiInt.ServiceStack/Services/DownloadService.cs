using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.MediaDelivery;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
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
		private readonly ILog _logger = LogManager.GetLogger("DownloadBestFormatService");

		public DownloadBestFormatService(IUrlSigner urlSigner, IOAuthCredentials configAuthCredentials, ICatalogue catalogue)
		{
			_urlSigner = urlSigner;
			_configAuthCredentials = configAuthCredentials;
			_catalogue = catalogue;
		}

		public HttpResult Get(DownloadRequest request)
		{
			var oAuthAccessToken = this.TryGetOAuthAccessToken();

			var url = BuildDownloadUrl(request);

			string signGetUrl = _urlSigner.SignGetUrl(url, oAuthAccessToken.Token, oAuthAccessToken.Secret, _configAuthCredentials);
			return new HttpResult
			{
				Headers = { { "Cache-control", "no-cache" } },
				Location = signGetUrl,
				StatusCode = HttpStatusCode.Redirect
			};
		}

		private string BuildDownloadUrl(DownloadRequest request)
		{
			var downloadUrl = request.Type == PurchaseType.release 
				? BuildReleaseUrl(request) 
				: BuildTrackUrl(request);

			//downloadUrl = downloadUrl + "&errorUrl=" + HttpUtility.UrlEncode(request.ErrorUrl);
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

		private string BuildTrackUrl(ItemRequest request)
		{
			var aTrack = _catalogue.GetATrack(request.CountryCode, request.Id);
			return string.Format("{0}?releaseid={1}&trackid={2}&country={3}",
										DownloadSettings.DOWNLOAD_TRACK_URL,
										aTrack.Release.Id,
										aTrack.Id,
										request.CountryCode);
		}
	}
}
