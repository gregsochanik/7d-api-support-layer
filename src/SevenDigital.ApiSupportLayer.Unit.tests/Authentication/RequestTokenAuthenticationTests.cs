using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;
using SevenDigital.Api.Wrapper.Http;
using SevenDigital.ApiSupportLayer.Authentication;

namespace SevenDigital.ApiSupportLayer.Unit.Tests.Authentication
{
	[TestFixture]
	public class RequestTokenAuthenticationTests
	{
		private const string EXPECTED_RESPONSE = "a test response";
		private IApiUri _apiUri;
		private IUrlSigner _urlSigner;
		private IOAuthCredentials _oAuthCredentials;
		private IHttpClient _httpClient;
		private readonly OAuthRequestToken _oAuthRequestToken = new OAuthRequestToken{Token = "TOKEN", Secret = "SECRET"};

		[SetUp]
		public void SetUp()
		{
			_apiUri = MockRepository.GenerateStub<IApiUri>();
			_urlSigner = MockRepository.GenerateStub<IUrlSigner>();
			_oAuthCredentials = MockRepository.GenerateStub<IOAuthCredentials>();
			_httpClient = MockRepository.GenerateStub<IHttpClient>();

			_httpClient.Stub(x => x.Post(null))
				.IgnoreArguments()
				.Return(new Response(HttpStatusCode.Accepted, EXPECTED_RESPONSE));
		}

		[Test]
		public void Should_return_response_of_a_post_to_the_Authentication_endpoint()
		{
			var requestTokenAuthentication = new RequestTokenAuthentication(_apiUri,_oAuthCredentials,_urlSigner,_httpClient);
			var authorise = requestTokenAuthentication.Authorise(_oAuthRequestToken, "test", "test");
			Assert.That(authorise.Body, Is.EqualTo(EXPECTED_RESPONSE));
		}

		[TearDown]
		public void TearDown()
		{}
	}
}