using ServiceStack.ServiceInterface;
using SevenDigital.ApiInt.Catalogue;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class ReleaseTracksService : Service
	{
		private readonly ICatalogue _catalogue;

		public ReleaseTracksService(ICatalogue catalogue)
		{
			_catalogue = catalogue;
		}

		public object Get(ReleaseTracksRequest itemRequest)
		{
			if (string.IsNullOrEmpty(itemRequest.CountryCode))
				itemRequest.CountryCode = "GB";

			return _catalogue.GetAReleaseTracks(itemRequest.CountryCode, itemRequest.Id);
		}
	}
}