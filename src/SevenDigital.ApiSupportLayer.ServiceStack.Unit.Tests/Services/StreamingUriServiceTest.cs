using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiSupportLayer.Authentication;
using SevenDigital.ApiSupportLayer.MediaDelivery;
using SevenDigital.ApiSupportLayer.ServiceStack.Services.Streaming;
using SevenDigital.ApiSupportLayer.TestData;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class StreamingUriServiceTest
	{
		private static readonly OAuthAccessToken _accessToken = new OAuthAccessToken
		{
			Token = FakeUserData.FakeAccessToken.Token,
			Secret = FakeUserData.FakeAccessToken.Secret
		};

		[Test]
		public void If_set_up_correctly_signs_the_correct_url()
		{
			var stubbedUrlSigner = MockRepository.GenerateStub<IOAuthSigner>();

			var streamingUriService = new StreamingUriService(stubbedUrlSigner)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};

			var streamingUrlRequest = new StreamingUrlRequest
			{
				Id = 12345
			};

			streamingUriService.Get(streamingUrlRequest);
			
			var parameters = new Dictionary<string, string>
			{
				{"trackid", streamingUrlRequest.Id.ToString()},
				{"formatid", StreamingSettings.CurrentStreamType.FormatId.ToString()},
				{"country", streamingUrlRequest.CountryCode},
			};

			var argumentsForCallsMadeOn = stubbedUrlSigner.GetArgumentsForCallsMadeOn(x => x.SignGetRequest(null, null, null), options => options.IgnoreArguments());
			Assert.That(argumentsForCallsMadeOn[0][0], Is.EqualTo(StreamingSettings.LOCKER_STREAMING_URL));

			var oAuthAccessToken = (OAuthAccessToken)(argumentsForCallsMadeOn[0][1]);
			Assert.That(oAuthAccessToken.Token, Is.EqualTo(_accessToken.Token));
			Assert.That(oAuthAccessToken.Secret, Is.EqualTo(_accessToken.Secret));

			var actualParams = (Dictionary<string, string>)(argumentsForCallsMadeOn[0][2]);

			Assert.That(actualParams["trackid"], Is.EqualTo(parameters["trackid"]));
			Assert.That(actualParams["formatid"], Is.EqualTo(parameters["formatid"]));
			Assert.That(actualParams["country"], Is.EqualTo(parameters["country"]));
		}

		[Test]
		public void Creates_valid_url_if_concrete_urlsigner_introduced()
		{
			var configAuthCredentials = MockRepository.GenerateStub<IOAuthCredentials>();
			configAuthCredentials.Stub(x => x.ConsumerKey).Return("ConsumerKey");
			configAuthCredentials.Stub(x => x.ConsumerSecret).Return("ConsumerSecret");
			var urlSigner =new CrennaOAuthSigner(configAuthCredentials);

			var streamingUriService = new StreamingUriService(urlSigner)
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