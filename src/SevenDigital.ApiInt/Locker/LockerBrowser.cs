using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Locker
{
	public class LockerBrowser : ILockerBrowser
	{
		private readonly IFluentApi<Api.Schema.LockerEndpoint.Locker> _lockerApi;
		private readonly ICatalogue _catalogue;

		public LockerBrowser(IFluentApi<Api.Schema.LockerEndpoint.Locker> lockerApi, ICatalogue catalogue)
		{
			_lockerApi = lockerApi;
			_catalogue = catalogue;
		}

		public LockerResponse GetLockerItem(OAuthAccessToken accessToken, ItemRequest lockerCheckRequest)
		{
			if (lockerCheckRequest.Type == PurchaseType.release)
			{
				return GetLockerItem(accessToken, lockerCheckRequest.Id);
			}

			var aTrack = _catalogue.GetATrack(lockerCheckRequest.CountryCode, lockerCheckRequest.Id);
			return GetLockerItem(accessToken, aTrack.Release.Id, lockerCheckRequest.Id);
		}

		public LockerResponse GetLockerItem(OAuthAccessToken accessToken, int releaseId, int trackId = 0)
		{
			var lockerApiCall = _lockerApi.ForUser(accessToken.Token, accessToken.Secret).ForReleaseId(releaseId).WithParameter("imagesize", "100");
			if (trackId > 0)
				lockerApiCall.ForTrackId(trackId);
			return lockerApiCall.Please().Response;
		}
	}
}