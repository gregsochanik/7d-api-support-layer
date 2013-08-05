using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Locker
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

		public LockerResponse GetLockerItem(OAuthAccessToken accessToken, ItemRequest request)
		{
			if (request.Type == PurchaseType.release)
			{
				return GetLockerItem(accessToken, request.Id);
			}

			if (!request.ReleaseId.HasValue)
			{
				var aTrack = _catalogue.GetATrack(request.CountryCode, request.Id);
				request.ReleaseId = aTrack.Release.Id;
			}
			return GetLockerItem(accessToken, request.ReleaseId.Value, request.Id);
		}

		private LockerResponse GetLockerItem(OAuthAccessToken accessToken, int releaseId, int trackId = 0)
		{
			var lockerApiCall = _lockerApi.ForUser(accessToken.Token, accessToken.Secret).ForReleaseId(releaseId).WithParameter("imagesize", "100");
			if (trackId > 0)
				lockerApiCall.ForTrackId(trackId);
			return lockerApiCall.Please().Response;
		}
	}
}