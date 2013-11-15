using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.Media;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.MediaDelivery;
using SevenDigital.ApiSupportLayer.Model;
using SevenDigital.ApiSupportLayer.ServiceStack.Model;
using SevenDigital.ApiSupportLayer.ServiceStack.Services;
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
		private static readonly string _expectedFakeToken = FakeUserData.FakeAccessToken.Token;
		private static readonly string _expectedFakeTokenSecret = FakeUserData.FakeAccessToken.Secret;

		private IOAuthCredentials _configAuthCredentials;
		private IUrlSigner _stubbedUrlSigner;
		private ICatalogue _stubbedCatalogue;

		[SetUp]
		public void SetUp()
		{
			_configAuthCredentials = MockRepository.GenerateStub<IOAuthCredentials>();
			_stubbedUrlSigner = MockRepository.GenerateStub<IUrlSigner>();
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
			var downloadTrackService = new DownloadFileService(_stubbedUrlSigner, _configAuthCredentials, _stubbedCatalogue)
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

			var expectedUrl = string.Format("{0}?releaseid={1}&trackid={2}&formatid={3}&country={4}", DownloadSettings.DOWNLOAD_TRACK_URL, EXPECTED_RELEASE_ID, EXPECTED_TRACK_ID, EXPECTED_FORMAT_ID, downloadTrackRequest.CountryCode);

			_stubbedUrlSigner.AssertWasCalled(x => x.SignGetUrl(expectedUrl, _expectedFakeToken, _expectedFakeTokenSecret, _configAuthCredentials));
		}

		[Test]
		public void Creates_valid_url_if_concrete_urlsigner_introduced()
		{
			_configAuthCredentials.Stub(x => x.ConsumerKey).Return("ConsumerKey");
			_configAuthCredentials.Stub(x => x.ConsumerSecret).Return("ConsumerSecret");
			var urlSigner = new UrlSigner(new OAuthSignatureGenerator());

			var downloadTrackService = new DownloadFileService(urlSigner, _configAuthCredentials, _stubbedCatalogue)
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
			var downloadTrackService = new DownloadFileService(_stubbedUrlSigner, _configAuthCredentials, _stubbedCatalogue)
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

			var expectedUrl = string.Format("{0}?releaseid={1}&formatid={2}&country={3}", DownloadSettings.DOWNLOAD_RELEASE_URL, EXPECTED_RELEASE_ID, EXPECTED_FORMAT_ID, downloadTrackRequest.CountryCode);

			_stubbedUrlSigner.AssertWasCalled(x => x.SignGetUrl(expectedUrl, _expectedFakeToken, _expectedFakeTokenSecret, _configAuthCredentials));
		}

		[Test]
		public void Creates_valid_url_if_concrete_urlsigner_introduced_release()
		{
			_configAuthCredentials.Stub(x => x.ConsumerKey).Return("ConsumerKey");
			_configAuthCredentials.Stub(x => x.ConsumerSecret).Return("ConsumerSecret");
			var urlSigner = new UrlSigner(new OAuthSignatureGenerator());

			var downloadTrackService = new DownloadFileService(urlSigner, _configAuthCredentials, _stubbedCatalogue)
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