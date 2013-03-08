using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.MediaDelivery;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class DownloadFileService : Service
	{
		private readonly IUrlSigner _urlSigner;
		private readonly IOAuthCredentials _configAuthCredentials;
		private readonly ICatalogue _catalogue;

		public DownloadFileService(IUrlSigner urlSigner, IOAuthCredentials configAuthCredentials, ICatalogue catalogue)
		{
			_urlSigner = urlSigner;
			_configAuthCredentials = configAuthCredentials;
			_catalogue = catalogue;
		}

		public HttpResult Get(DownloadTrackRequest request)
		{
			var oAuthAccessToken = this.TryGetOAuthAccessToken();

			var url = BuildDownloadUrl(request);

			return new HttpResult
			{
				Headers = { { "Cache-control", "no-cache" } },
				Location = _urlSigner.SignGetUrl(url, oAuthAccessToken.Token, oAuthAccessToken.Secret, _configAuthCredentials),
				StatusCode = HttpStatusCode.Redirect
			};
		}

		private string BuildDownloadUrl(DownloadTrackRequest request)
		{
			if (request.Type == PurchaseType.release)
			{
				return string.Format("{0}?releaseid={1}&formatid={2}&country={3}",
									DownloadSettings.DOWNLOAD_RELEASE_URL,
									request.Id,
									request.FormatId,
									request.CountryCode);
			}

			var aTrack = _catalogue.GetATrack(request.CountryCode, request.Id);
			return string.Format("{0}?releaseid={1}&trackid={2}&formatid={3}&country={4}",
								DownloadSettings.DOWNLOAD_TRACK_URL,
								aTrack.Release.Id,
								aTrack.Id,
								request.FormatId,
								request.CountryCode);
		}
	}
}