using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;
using SevenDigital.ApiSupportLayer.MediaDelivery;
using SevenDigital.ApiSupportLayer.ServiceStack.Model;
using SevenDigital.ApiSupportLayer.ServiceStack.Services;
using SevenDigital.ApiSupportLayer.TestData;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class StreamingUriServiceTest
	{
		private static readonly string _userToken = FakeUserData.FakeAccessToken.Token;
		private static readonly string _tokenSecret = FakeUserData.FakeAccessToken.Secret;

		[Test]
		public void If_set_up_correctly_signs_the_correct_url()
		{
			var configAuthCredentials = MockRepository.GenerateStub<IOAuthCredentials>();
			
			var stubbedUrlSigner = MockRepository.GenerateStub<IUrlSigner>();

			var streamingUriService = new StreamingUriService(stubbedUrlSigner, configAuthCredentials)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};

			var streamingUrlRequest = new StreamingUrlRequest
			{
				Id = 12345
			};

			streamingUriService.Get(streamingUrlRequest);

			var expectedUrl = string.Format("{0}?trackid={1}&formatid={2}&country={3}", StreamingSettings.LOCKER_STREAMING_URL, 12345, StreamingSettings.CurrentStreamType.FormatId, streamingUrlRequest.CountryCode);

			stubbedUrlSigner.AssertWasCalled(x => x.SignGetUrl(expectedUrl, _userToken, _tokenSecret, configAuthCredentials));
		}

		[Test]
		public void Creates_valid_url_if_concrete_urlsigner_introduced()
		{
			var configAuthCredentials = MockRepository.GenerateStub<IOAuthCredentials>();
			configAuthCredentials.Stub(x => x.ConsumerKey).Return("ConsumerKey");
			configAuthCredentials.Stub(x => x.ConsumerSecret).Return("ConsumerSecret");
			var urlSigner = new UrlSigner();

			var streamingUriService = new StreamingUriService(urlSigner, configAuthCredentials)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};

			var streamingUrlRequest = new StreamingUrlRequest
			{
				Id = 12345
			};

			var s = streamingUriService.Get(streamingUrlRequest);

			Assert.That(s.Headers["Location"], Is.StringContaining(StreamingSettings.LOCKER_STREAMING_URL));
			Assert.That(s.Headers["Cache-control"], Is.EqualTo("no-cache"));
			Assert.That(s.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
		}
	}
}