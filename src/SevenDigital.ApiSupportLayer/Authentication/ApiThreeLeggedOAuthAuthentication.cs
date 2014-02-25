using System;
using System.Xml;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Http;
using SevenDigital.Api.Wrapper.Responses;

namespace SevenDigital.ApiSupportLayer.Authentication
{
	public class ApiThreeLeggedOAuthAuthentication : IOAuthAuthentication
	{
		private readonly IFluentApi<OAuthRequestToken> _requestTokenApiCall;
		private readonly IRequestTokenAuthentication _requestTokenAuthentication;
		private readonly IFluentApi<OAuthAccessToken> _accessTokenApiCall;

		public ApiThreeLeggedOAuthAuthentication(IFluentApi<OAuthRequestToken> requestTokenApiCall, IRequestTokenAuthentication requestTokenAuthentication, IFluentApi<OAuthAccessToken> accessTokenApiCall)
		{
			_requestTokenApiCall = requestTokenApiCall;
			_requestTokenAuthentication = requestTokenAuthentication;
			_accessTokenApiCall = accessTokenApiCall;
		}

		public OAuthAccessToken ForUser(string username, string password)
		{
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
				throw new ArgumentException("username and password must both contain a value");

			return GetOAuthAccessTokenFrom7digital(username, password);
		}

		private OAuthAccessToken GetOAuthAccessTokenFrom7digital(string username, string password)
		{
			var oAuthRequestToken = _requestTokenApiCall.Please();

			var response = _requestTokenAuthentication.Authorise(oAuthRequestToken, username, password);

			ConfirmUserExists(response);

			return _accessTokenApiCall.ForUser(oAuthRequestToken.Token, oAuthRequestToken.Secret).Please();
		}

		private static void ConfirmUserExists(Response authoriseRequestToken)
		{
			try
			{
				CheckXmlForError(authoriseRequestToken.Body);
			}
			catch (XmlException)
			{
				HandleNonXmlResponse(authoriseRequestToken.Body);
			}
		}

		private static void CheckXmlForError(string body)
		{
			var xml = new XmlDocument();
			xml.LoadXml(body);
			var responseNode = xml.SelectSingleNode("/response");
			if (responseNode.Attributes["status"].Value == "error")
			{
				throw new LoginInvalidException();
			}
		}

		private static void HandleNonXmlResponse(string body)
		{
			throw new LoginInvalidException(body);
		}
	}
}