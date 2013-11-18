using System;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Http;
using SevenDigital.ApiSupportLayer.Authentication;

namespace SevenDigital.ApiSupportLayer.Unit.Tests.Authentication
{
	[TestFixture]
	public class ApiThreeLeggedOAuthAuthenticationTests
	{
		private ApiThreeLeggedOAuthAuthentication _apiThreeLeggedOAuthAuthentication;
		private readonly OAuthRequestToken _oAuthRequestToken = new OAuthRequestToken
		{
			Token = "REQUEST_TOKEN",
			Secret = "SECRET"
		};

		private readonly OAuthAccessToken _oAuthAccessToken = new OAuthAccessToken
		{
			Token = "TOKEN",
			Secret = "SECRET"
		};

		private IFluentApi<OAuthRequestToken> _requestTokenApiCall;
		private IRequestTokenAuthentication _requestTokenAuthentication;
		private IFluentApi<OAuthAccessToken> _accessTokenApiCall;

		private const string RESPONSE_STATUS_OK = "<response status=\"ok\"/>";
		private const string RESPONSE_STATUS_ERROR = "<response status=\"error\"/>";
		private const string RESPONSE_STATUS_NON_XML = "ba-jeebus";

		[SetUp]
		public void SetUp()
		{
			_requestTokenApiCall = MockRepository.GenerateStub<IFluentApi<OAuthRequestToken>>();
			_requestTokenApiCall.Stub(x => x.Please()).Return(_oAuthRequestToken);

			_requestTokenAuthentication = MockRepository.GenerateStub<IRequestTokenAuthentication>();
			_requestTokenAuthentication.Stub(x => x.Authorise(null, "", ""))
				.IgnoreArguments()
				.Return(new Response(HttpStatusCode.OK, RESPONSE_STATUS_OK));

			_accessTokenApiCall = MockRepository.GenerateStub<IFluentApi<OAuthAccessToken>>();
			_accessTokenApiCall.Stub(x => x.ForUser("", "")).IgnoreArguments().Return(_accessTokenApiCall);
			_accessTokenApiCall.Stub(x => x.Please()).IgnoreArguments().Return(_oAuthAccessToken);

			_apiThreeLeggedOAuthAuthentication = new ApiThreeLeggedOAuthAuthentication(_requestTokenApiCall, _requestTokenAuthentication, _accessTokenApiCall);
		}

		[Test]
		public void Throws_if_username_blank()
		{
			Assert.Throws<ArgumentException>(() => _apiThreeLeggedOAuthAuthentication.ForUser("", "password"));
		}

		[Test]
		public void Throws_if_password_blank()
		{
			Assert.Throws<ArgumentException>(() => _apiThreeLeggedOAuthAuthentication.ForUser("user", ""));
		}

		[Test]
		public void Happy_path_if_xml_response_from_Authoroise_call_is_ok()
		{
			var oAuthAccessToken = _apiThreeLeggedOAuthAuthentication.ForUser("test", "test");
			Assert.That(oAuthAccessToken, Is.EqualTo(_oAuthAccessToken));
		}

		[Test]
		public void throws_LoginInvalidException_if_xml_response_from_Authorise_call_is_an_error()
		{
			var requestTokenAuthentication = MockRepository.GenerateStub<IRequestTokenAuthentication>();
			requestTokenAuthentication.Stub(x => x.Authorise(null, "", ""))
				.IgnoreArguments()
				.Return(new Response(HttpStatusCode.OK, RESPONSE_STATUS_ERROR));

			var apiThreeLeggedOAuthAuthentication = new ApiThreeLeggedOAuthAuthentication(_requestTokenApiCall, requestTokenAuthentication, _accessTokenApiCall);

			Assert.Throws<LoginInvalidException>(() => apiThreeLeggedOAuthAuthentication.ForUser("test", "test"));
		}

		[Test]
		public void throws_LoginInvalidException_with_body_if_response_from_Authorise_call_is_non_xml()
		{
			var requestTokenAuthentication = MockRepository.GenerateStub<IRequestTokenAuthentication>();
			requestTokenAuthentication.Stub(x => x.Authorise(null, "", ""))
				.IgnoreArguments()
				.Return(new Response(HttpStatusCode.OK, RESPONSE_STATUS_NON_XML));

			var apiThreeLeggedOAuthAuthentication = new ApiThreeLeggedOAuthAuthentication(_requestTokenApiCall, requestTokenAuthentication, _accessTokenApiCall);

			var loginInvalidException = Assert.Throws<LoginInvalidException>(() => apiThreeLeggedOAuthAuthentication.ForUser("test", "test"));
			Assert.That(loginInvalidException.Message, Is.EqualTo(RESPONSE_STATUS_NON_XML));
		}
	}
}
