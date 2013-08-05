using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
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
}