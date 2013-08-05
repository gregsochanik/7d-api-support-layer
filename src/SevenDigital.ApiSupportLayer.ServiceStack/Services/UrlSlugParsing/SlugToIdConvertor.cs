using System;
using System.Linq;
using SevenDigital.Api.Schema.ArtistEndpoint;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.UrlSlugParsing
{
	public class SlugToIdConvertor
	{
		private readonly IFluentApi<ArtistSearch> _artistSearchApi;
		private readonly IFluentApi<ReleaseSearch> _releaseSearchApi;

		public SlugToIdConvertor(IFluentApi<ArtistSearch> artistSearchApi, IFluentApi<ReleaseSearch> releaseSearchApi)
		{
			_artistSearchApi = artistSearchApi;
			_releaseSearchApi = releaseSearchApi;
		}

		public int ArtistIdFromSlug(string slug)
		{
			throw new NotImplementedException();
			var artistSearch = _artistSearchApi.WithAdvancedQuery("artistUrl:" + slug).Please();
			return artistSearch.Results.First().Artist.Id;
		}

		public int ReleaseIdFromSlug(string slug)
		{
			var releaseSearch = _releaseSearchApi.WithAdvancedQuery("productUrl:" + slug).Please();

			if(releaseSearch.TotalItems < 1)
				throw new SlugNotFoundException(slug);

			return releaseSearch.Results.First().Release.Id;
		}

		public Release ReleaseFromSlug(string slug)
		{
			var releaseSearch = _releaseSearchApi.WithAdvancedQuery("productUrl:" + slug).Please();

			if (releaseSearch.TotalItems < 1)
				throw new SlugNotFoundException(slug);

			return releaseSearch.Results.First().Release;
		}
	}
}