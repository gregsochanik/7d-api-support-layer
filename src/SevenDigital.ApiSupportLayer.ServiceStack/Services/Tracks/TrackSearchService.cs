using System.Linq;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.Tracks
{
	public class TrackSearchService : Service
	{
		private readonly IFluentApi<TrackSearch> _trachSearch;

		public TrackSearchService(IFluentApi<TrackSearch> trachSearch)
		{
			_trachSearch = trachSearch;
		}

		public object Get(TrackSearchRequest trackSearchRequest)
		{
			if (trackSearchRequest.PageSize < 1)
				trackSearchRequest.PageSize = 10;

			if (string.IsNullOrEmpty(trackSearchRequest.CountryCode))
				trackSearchRequest.CountryCode = "GB";

			var releaseTracks = _trachSearch.WithParameter("country", trackSearchRequest.CountryCode)
			                                .WithQuery(trackSearchRequest.Query)
											.WithParameter("imagesize", "50")
			                                .WithPageSize(trackSearchRequest.PageSize)
			                                .Please();

			return releaseTracks.Results.Select(x=>x.Track).ToList();
		}
	}
}