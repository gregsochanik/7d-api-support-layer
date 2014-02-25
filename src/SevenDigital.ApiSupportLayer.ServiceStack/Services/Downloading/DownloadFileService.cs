using System.Collections.Generic;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.ApiSupportLayer.Authentication;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.MediaDelivery;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.Downloading
{
	public class DownloadFileService : Service
	{
		private readonly IOAuthSigner _urlSigner;
		private readonly ICatalogue _catalogue;

		public DownloadFileService(IOAuthSigner urlSigner, ICatalogue catalogue)
		{
			_urlSigner = urlSigner;
			_catalogue = catalogue;
		}

		public HttpResult Get(DownloadTrackRequest request)
		{
			var oAuthAccessToken = this.TryGetOAuthAccessToken();

			var url = request.Type == PurchaseType.release 
				? DownloadSettings.DOWNLOAD_RELEASE_URL 
				: DownloadSettings.DOWNLOAD_TRACK_URL;

			var parameters = BuildDownloadUrl(request);

			return new HttpResult
			{
				Headers = { { "Cache-control", "no-cache" } },
				Location = _urlSigner.SignGetRequest(url, oAuthAccessToken, parameters),
				StatusCode = HttpStatusCode.Redirect
			};
		}

		private Dictionary<string,string> BuildDownloadUrl(DownloadTrackRequest request)
		{
			if (request.Type == PurchaseType.release)
			{
				return new Dictionary<string, string>
				{
					{"releaseId", request.Id.ToString()},
					{"formatid", request.FormatId.ToString()},
					{"country", request.CountryCode},
				};
			}

			var aTrack = _catalogue.GetATrack(request.CountryCode, request.Id);
			return new Dictionary<string, string>
			{
				{"releaseId", aTrack.Release.Id.ToString()},
				{"trackid", aTrack.Id.ToString()},
				{"formatid", request.FormatId.ToString()},
				{"country", request.CountryCode},
			};
		}
	}
}