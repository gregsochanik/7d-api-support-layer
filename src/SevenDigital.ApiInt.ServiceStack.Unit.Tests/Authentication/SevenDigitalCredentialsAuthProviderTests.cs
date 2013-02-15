using System.Net;
using System.Web;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Testing;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiInt.Authentication;
using SevenDigital.ApiInt.ServiceStack.Authentication;
using SevenDigital.ApiInt.User;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Authentication
{
	[TestFixture]
	public class _if_user_exists
	{
		private IUserApi _userApi;
		private IOAuthAuthentication _oAuthAuthentication;

		[SetUp]
		public void SetUp()
		{
			_userApi = MockRepository.GenerateStub<IUserApi>();
			_userApi.Stub(x => x.CheckUserExists("")).IgnoreArguments().Return(true);
			_oAuthAuthentication = MockRepository.GenerateStub<IOAuthAuthentication>();
		}

		[Test]
		public void Calls_api_authentication_class()
		{
			var oAuthAccessToken = new OAuthAccessToken();

			_oAuthAuthentication.Stub(x => x.ForUser("test", "test")).Return(oAuthAccessToken);
			var sevenDigitalCredentialsAuthProvider = new SevenDigitalCredentialsAuthProvider(_oAuthAuthentication, _userApi);

			var authService = MockRepository.GenerateStub<IServiceBase>();

			authService.Stub(x => x.RequestContext).Return(new MockRequestContext());

			var isAuthenticated = sevenDigitalCredentialsAuthProvider.TryAuthenticate(authService, "test", "test");
			_oAuthAuthentication.AssertWasCalled(x => x.ForUser("test", "test"));
			Assert.That(isAuthenticated);
		}

		[Test]
		public void Throws_httpError_if_fails()
		{
			_oAuthAuthentication.Stub(x => x.ForUser("test", "test")).Throw(new LoginInvalidException(""));
			var sevenDigitalCredentialsAuthProvider = new SevenDigitalCredentialsAuthProvider(_oAuthAuthentication, _userApi);
			var serviceBase = MockRepository.GenerateStub<IServiceBase>();

			var httpError = Assert.Throws<HttpError>(() => sevenDigitalCredentialsAuthProvider.TryAuthenticate(serviceBase, "test", "test"));
			Assert.That(httpError.ErrorCode, Is.EqualTo("Login invalid"));
			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
		}
	}

	[TestFixtureAttribute]
	public class _if_user_does_not_exist
	{
		private readonly IUserApi _userApi = MockRepository.GenerateStub<IUserApi>();
		private readonly IOAuthAuthentication _oAuthAuthentication = MockRepository.GenerateStub<IOAuthAuthentication>();

		[SetUp]
		public void SetUp()
		{
			_userApi.Stub(x => x.CheckUserExists("")).IgnoreArguments().Return(false);
		}

		[Test]
		public void Fires_create_method_and_then_logs_in()
		{
			const string expectedUsername = "username@thing.com";
			const string expectedPassword = "password!";

			var oAuthAccessToken = new OAuthAccessToken();

			_oAuthAuthentication.Stub(x => x.ForUser(null, null)).IgnoreArguments().Return(oAuthAccessToken);

			var serviceBase = MockRepository.GenerateStub<IServiceBase>();
			serviceBase.Stub(x => x.RequestContext).Return(new MockRequestContext());
			var sevenDigitalCredentialsAuthProvider = new SevenDigitalCredentialsAuthProvider(_oAuthAuthentication, _userApi);

			sevenDigitalCredentialsAuthProvider.TryAuthenticate(serviceBase, expectedUsername, expectedPassword);
			_userApi.AssertWasCalled(x => x.Create(expectedUsername, expectedPassword));
			_oAuthAuthentication.AssertWasCalled(x => x.ForUser(HttpUtility.UrlEncode(expectedUsername), HttpUtility.UrlEncode(expectedPassword)));
		}
	}
}
