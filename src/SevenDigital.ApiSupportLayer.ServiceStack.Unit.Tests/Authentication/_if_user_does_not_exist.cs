using System.Net;
using System.Web;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Testing;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiSupportLayer.Authentication;
using SevenDigital.ApiSupportLayer.ServiceStack.Authentication;
using SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Services;
using SevenDigital.ApiSupportLayer.User;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Authentication
{
	[TestFixture]
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
			_userApi.AssertWasCalled(x => x.Create(expectedUsername, expectedPassword, ""));
			_oAuthAuthentication.AssertWasCalled(x => x.ForUser(HttpUtility.UrlEncode(expectedUsername), HttpUtility.UrlEncode(expectedPassword)));
		}


		[Test]
		public void Fires_create_method_and_passes_down_affiliate_id_if_exists()
		{
			const string expectedUsername = "username@thing.com";
			const string expectedPassword = "password!";
			const string expectedAffiliatePartnerId = "712";

			var oAuthAccessToken = new OAuthAccessToken();

			_oAuthAuthentication.Stub(x => x.ForUser(null, null)).IgnoreArguments().Return(oAuthAccessToken);

			var serviceBase = MockRepository.GenerateStub<IServiceBase>();

			var loggedInContext = ContextHelper.LoggedInContext();
			var httpReq = (MockHttpRequest)loggedInContext.Get<IHttpRequest>();
			httpReq.Headers = new WebHeaderCollection
			{
				{"X-7d-partner-id", expectedAffiliatePartnerId}
			};

			serviceBase.Stub(x => x.RequestContext).Return(loggedInContext);
			var sevenDigitalCredentialsAuthProvider = new SevenDigitalCredentialsAuthProvider(_oAuthAuthentication, _userApi);

			sevenDigitalCredentialsAuthProvider.TryAuthenticate(serviceBase, expectedUsername, expectedPassword);
			_userApi.AssertWasCalled(x => x.Create(expectedUsername, expectedPassword, expectedAffiliatePartnerId));
			_oAuthAuthentication.AssertWasCalled(x => x.ForUser(HttpUtility.UrlEncode(expectedUsername), HttpUtility.UrlEncode(expectedPassword)));
		}
	}
}