using System.Net;
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
			_oAuthAuthentication.Stub(x => x.ForUser("test", "test")).Throw(new LoginInvalidException());
			var sevenDigitalCredentialsAuthProvider = new SevenDigitalCredentialsAuthProvider(_oAuthAuthentication, _userApi);
			var serviceBase = MockRepository.GenerateStub<IServiceBase>();

			var httpError = Assert.Throws<HttpError>(() => sevenDigitalCredentialsAuthProvider.TryAuthenticate(serviceBase, "test", "test"));
			Assert.That(httpError.ErrorCode, Is.EqualTo("Login invalid"));
			Assert.That(httpError.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
		}
	}
}
