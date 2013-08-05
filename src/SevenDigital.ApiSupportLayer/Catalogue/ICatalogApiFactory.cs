using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiInt.Catalogue
{
	public interface ICatalogApiFactory
	{
		IFluentApi<Track> TrackApi();
		IFluentApi<Release> ReleaseApi();
		IFluentApi<ReleaseTracks> ReleaseTracksApi();
	}
}