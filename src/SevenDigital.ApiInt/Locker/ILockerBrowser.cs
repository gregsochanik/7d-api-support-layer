using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Locker
{
	public interface ILockerBrowser
	{
		//LockerResponse GetLockerItem(OAuthAccessToken accessToken, int releaseId, int trackId = 0);
		LockerResponse GetLockerItem(OAuthAccessToken accessToken, ItemRequest request);
	}
}