using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Locker
{
	public interface ILockerBrowser
	{
		//LockerResponse GetLockerItem(OAuthAccessToken accessToken, int releaseId, int trackId = 0);
		LockerResponse GetLockerItem(OAuthAccessToken accessToken, ItemRequest request);
	}
}