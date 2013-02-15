using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;
using SevenDigital.ApiInt.MediaDelivery;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class StreamingUriService : Service
	{
		private readonly IUrlSigner _urlSigner;
		private readonly IOAuthCredentials _configAuthCredentials;

		public StreamingUriService(IUrlSigner urlSigner, IOAuthCredentials configAuthCredentials)
		{
			_urlSigner = urlSigner;
			_configAuthCredentials = configAuthCredentials;
		}

		public HttpResult Get(StreamingUrlRequest request)
		{
			const string streamingEndpoint = StreamingSettings.LOCKER_STREAMING_URL;

			var oAuthAccessToken = this.TryGetOAuthAccessToken();

			var formatId = StreamingSettings.CurrentStreamType.FormatId;

			var url = string.Format("{0}?trackid={1}&formatid={2}&country={3}", streamingEndpoint, request.Id, formatId, request.CountryCode);
			
			return new HttpResult
			{
				Headers = {{ "Cache-control", "no-cache" }},
				Location = _urlSigner.SignGetUrl(url, oAuthAccessToken.Token, oAuthAccessToken.Secret, _configAuthCredentials),
				StatusCode = HttpStatusCode.Redirect
			};
		}
	}
}