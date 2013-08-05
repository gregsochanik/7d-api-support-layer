using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class ReleaseRecommendService : Service
	{
		private readonly IFluentApi<ReleaseRecommend> _recommendApi;

		public ReleaseRecommendService(IFluentApi<ReleaseRecommend> recommendApi)
		{
			_recommendApi = recommendApi;
		}

		public ReleaseRecommend Get(ReleaseRecommendRequest request)
		{
			var forReleaseId = _recommendApi.ForReleaseId(request.ReleaseId);

			if (request.Page > 0)
			{
				forReleaseId.WithPageNumber(request.Page);
			}

			if (request.PageSize > 0)
			{
				forReleaseId.WithPageNumber(request.PageSize);
			}

			return forReleaseId.Please();
		}
	}
}