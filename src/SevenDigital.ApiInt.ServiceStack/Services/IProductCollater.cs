using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public interface IProductCollater
	{
		ReleaseAndTracks UsingReleaseId(string countryCode, int releaseId);
		ReleaseAndTracks UsingTrackId(string countryCode, int trackId);
		ReleaseAndTracks UsingReleaseAndTrackId(string countryCode, int releaseId, int trackId);
	}
}