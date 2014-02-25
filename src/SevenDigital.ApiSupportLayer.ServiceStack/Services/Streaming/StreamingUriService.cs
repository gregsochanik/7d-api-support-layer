using System.Collections.Generic;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.ApiSupportLayer.Authentication;
using SevenDigital.ApiSupportLayer.MediaDelivery;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.Streaming
{
	public class StreamingUriService : Service
	{
		private readonly IOAuthSigner _urlSigner;

		public StreamingUriService(IOAuthSigner urlSigner)
		{
			_urlSigner = urlSigner;
		}

		public HttpResult Get(StreamingUrlRequest request)
		{
			var oAuthAccessToken = this.TryGetOAuthAccessToken();

			var parameters = new Dictionary<string, string>
			{
				{"trackid", request.Id.ToString()},
				{"formatid", StreamingSettings.CurrentStreamType.FormatId.ToString()},
				{"country", request.CountryCode},
			};
			return new HttpResult
			{
				Headers = {{ "Cache-control", "no-cache" }},
				Location = _urlSigner.SignGetRequest(StreamingSettings.LOCKER_STREAMING_URL, oAuthAccessToken, parameters),
				StatusCode = HttpStatusCode.Redirect
			};
		}
	}
}