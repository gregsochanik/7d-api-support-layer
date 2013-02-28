using System.Linq;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
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

	public class TrackService : Service
	{
		private readonly ICatalogue _catalogue;

		public TrackService(ICatalogue catalogue)
		{
			_catalogue = catalogue;
		}

		public Track Get(TrackRequest request)
		{
			var aTrackWithPrice = _catalogue.GetATrackWithPrice(request.CountryCode, request.Id);
			return aTrackWithPrice;
		}
	}

	public class TrackRequest : ItemRequest
	{}
}