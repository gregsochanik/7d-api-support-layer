using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceStack.Common.Web;
using SevenDigital.Api.Schema.Media;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiSupportLayer.Authentication;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.MediaDelivery;
using SevenDigital.ApiSupportLayer.Model;
using SevenDigital.ApiSupportLayer.ServiceStack.Services.Downloading;
using SevenDigital.ApiSupportLayer.TestData;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class DownloadFileServiceTests
	{
		private const int EXPECTED_TRACK_ID = 12345;
		private const int EXPECTED_RELEASE_ID = 54321;
		private const int EXPECTED_FORMAT_ID = 1;

		private static readonly OAuthAccessToken _accessToken = new OAuthAccessToken
		{
			Token = FakeUserData.FakeAccessToken.Token,
			Secret = FakeUserData.FakeAccessToken.Secret
		};

		private IOAuthCredentials _configAuthCredentials;
		private IOAuthSigner _stubbedUrlSigner;
		private ICatalogue _stubbedCatalogue;

		[SetUp]
		public void SetUp()
		{
			_configAuthCredentials = MockRepository.GenerateStub<IOAuthCredentials>();
			_stubbedUrlSigner = MockRepository.GenerateStub<IOAuthSigner>();
			_stubbedCatalogue = MockRepository.GenerateStub<ICatalogue>();
			_stubbedCatalogue.Stub(x => x.GetATrack(null, 0)).IgnoreArguments().Return(new Track
			{
				Id = EXPECTED_TRACK_ID,
				Release = new Release
				{
					Id = EXPECTED_RELEASE_ID,
					Formats = new FormatList { Formats = new List<Format> { new Format { Id = EXPECTED_FORMAT_ID } } }
				}
			});
			_stubbedCatalogue.Stub(x => x.GetARelease(null, 0)).IgnoreArguments().Return(new Release
			{
				Id = EXPECTED_TRACK_ID,
				Formats = new FormatList { Formats = new List<Format> { new Format { Id = EXPECTED_FORMAT_ID } } }
			});

		}

		[Test]
		public void If_set_up_correctly_signs_the_correct_url()
		{
			var downloadTrackService = new DownloadFileService(_stubbedUrlSigner, _stubbedCatalogue)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};

			var downloadTrackRequest = new DownloadTrackRequest
			{
				Id = EXPECTED_TRACK_ID,
				Type = PurchaseType.track,
				FormatId = EXPECTED_FORMAT_ID
			};

			downloadTrackService.Get(downloadTrackRequest);

			var parameters = new Dictionary<string, string>
			{
				{"releaseId", EXPECTED_RELEASE_ID.ToString()},
				{"trackid", EXPECTED_TRACK_ID.ToString()},
				{"formatid",EXPECTED_FORMAT_ID.ToString()},
				{"country", downloadTrackRequest.CountryCode},
			};

			var argumentsForCallsMadeOn = _stubbedUrlSigner.GetArgumentsForCallsMadeOn(x => x.SignGetRequest(null, null, null), options => options.IgnoreArguments());
			Assert.That(argumentsForCallsMadeOn[0][0], Is.EqualTo(DownloadSettings.DOWNLOAD_TRACK_URL));

			var oAuthAccessToken = (OAuthAccessToken) (argumentsForCallsMadeOn[0][1]);
			Assert.That(oAuthAccessToken.Token, Is.EqualTo(_accessToken.Token));
			Assert.That(oAuthAccessToken.Secret, Is.EqualTo(_accessToken.Secret));

			var actualParams = (Dictionary<string, string>)(argumentsForCallsMadeOn[0][2]);

			Assert.That(actualParams["releaseId"], Is.EqualTo(parameters["releaseId"]));
			Assert.That(actualParams["trackid"], Is.EqualTo(parameters["trackid"]));
			Assert.That(actualParams["formatid"], Is.EqualTo(parameters["formatid"]));
			Assert.That(actualParams["country"], Is.EqualTo(parameters["country"]));

		}

		[Test]
		public void Creates_valid_url_if_concrete_urlsigner_introduced()
		{
			_configAuthCredentials.Stub(x => x.ConsumerKey).Return("ConsumerKey");
			_configAuthCredentials.Stub(x => x.ConsumerSecret).Return("ConsumerSecret");
			var urlSigner = new CrennaOAuthSigner(_configAuthCredentials);

			var downloadTrackService = new DownloadFileService(urlSigner, _stubbedCatalogue)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};

			var downloadTrackRequest = new DownloadTrackRequest
			{
				Id = EXPECTED_TRACK_ID,
				Type = PurchaseType.track
			};

			var s = downloadTrackService.Get(downloadTrackRequest);

			Assert.That(s.Headers["Location"], Is.StringContaining(DownloadSettings.DOWNLOAD_TRACK_URL));
			Assert.That(s.Headers["Cache-control"], Is.EqualTo("no-cache"));
			Assert.That(s.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
		}

		[Test]
		public void If_set_up_correctly_signs_the_correct_url_release()
		{
			var downloadTrackService = new DownloadFileService(_stubbedUrlSigner,  _stubbedCatalogue)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};

			var downloadTrackRequest = new DownloadTrackRequest
			{
				Id = EXPECTED_RELEASE_ID,
				Type = PurchaseType.release,
				FormatId = EXPECTED_FORMAT_ID
			};

			downloadTrackService.Get(downloadTrackRequest);
			
			var parameters = new Dictionary<string, string>
			{
				{"releaseId", EXPECTED_RELEASE_ID.ToString()},
				{"formatid", EXPECTED_FORMAT_ID.ToString()},
				{"country", downloadTrackRequest.CountryCode},
			};

			var argumentsForCallsMadeOn = _stubbedUrlSigner.GetArgumentsForCallsMadeOn(x => x.SignGetRequest(null, null, null), options => options.IgnoreArguments());
			Assert.That(argumentsForCallsMadeOn[0][0], Is.EqualTo(DownloadSettings.DOWNLOAD_RELEASE_URL));

			var oAuthAccessToken = (OAuthAccessToken)(argumentsForCallsMadeOn[0][1]);
			Assert.That(oAuthAccessToken.Token, Is.EqualTo(_accessToken.Token));
			Assert.That(oAuthAccessToken.Secret, Is.EqualTo(_accessToken.Secret));

			var actualParams = (Dictionary<string, string>)(argumentsForCallsMadeOn[0][2]);

			Assert.That(actualParams["releaseId"], Is.EqualTo(parameters["releaseId"]));
			Assert.That(actualParams["formatid"], Is.EqualTo(parameters["formatid"]));
			Assert.That(actualParams["country"], Is.EqualTo(parameters["country"]));
		}

		[Test]
		public void Creates_valid_url_if_concrete_urlsigner_introduced_release()
		{
			_configAuthCredentials.Stub(x => x.ConsumerKey).Return("ConsumerKey");
			_configAuthCredentials.Stub(x => x.ConsumerSecret).Return("ConsumerSecret");
			var urlSigner = new CrennaOAuthSigner(_configAuthCredentials);

			var downloadTrackService = new DownloadFileService(urlSigner, _stubbedCatalogue)
			{
				RequestContext = ContextHelper.LoggedInContext()
			};

			var downloadTrackRequest = new DownloadTrackRequest
			{
				Id = EXPECTED_TRACK_ID,
				Type = PurchaseType.release
			};

			var s = downloadTrackService.Get(downloadTrackRequest);

			Assert.That(s.Headers["Location"], Is.StringContaining(DownloadSettings.DOWNLOAD_RELEASE_URL));
			Assert.That(s.Headers["Cache-control"], Is.EqualTo("no-cache"));
			Assert.That(s.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
		}
	}
}