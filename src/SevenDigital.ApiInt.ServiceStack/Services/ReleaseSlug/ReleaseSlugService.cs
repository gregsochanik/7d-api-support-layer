using ServiceStack.ServiceInterface;
using SevenDigital.ApiInt.ServiceStack.Services.UrlSlugParsing;

namespace SevenDigital.ApiInt.ServiceStack.Services.ReleaseSlug
{
	public class ReleaseSlugService : Service
	{
		private readonly SlugToIdConvertor _slugToIdConvertor;

		public ReleaseSlugService(SlugToIdConvertor slugToIdConvertor)
		{
			_slugToIdConvertor = slugToIdConvertor;
		}

		public ReleaseSlugResponse Get(ReleaseSlugRequest request)
		{
			var release = _slugToIdConvertor.ReleaseFromSlug(request.ReleaseSlug);

			return new ReleaseSlugResponse { OriginalRequest = request, Release = release };
		}
	}
}