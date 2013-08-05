using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Catalogue
{
	public interface IProductCollater
	{
		ReleaseAndTracks UsingReleaseId(string countryCode, int releaseId);
		ReleaseAndTracks UsingTrackId(string countryCode, int trackId);
		ReleaseAndTracks UsingReleaseAndTrackId(string countryCode, int releaseId, int trackId);
	}
}