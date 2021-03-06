using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiSupportLayer.Locker
{
	public interface ILockerRetrieval
	{
		IEnumerable<LockerRelease> GetLockerReleases(OAuthAccessToken accessToken, LockerStats lockerStats);
		LockerStats GetLockerStats(OAuthAccessToken accessToken);
	}
}