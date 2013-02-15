using System.Collections.Generic;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;

namespace SevenDigital.ApiInt.Catalogue
{
	public interface ICatalogue
	{
		Track GetATrack(string countryCode, int id);
		Track GetATrackWithPrice(string countryCode, int id);
		Release GetARelease(string countryCode, int id);
		List<Track> GetAReleaseTracks(string countryCode, int id);
	}
}