using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.Tracks
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