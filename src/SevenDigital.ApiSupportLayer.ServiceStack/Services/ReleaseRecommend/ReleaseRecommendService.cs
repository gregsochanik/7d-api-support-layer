using ServiceStack.ServiceInterface;
using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.ReleaseRecommend
{
	public class ReleaseRecommendService : Service
	{
		private readonly IFluentApi<Api.Schema.ReleaseEndpoint.ReleaseRecommend> _recommendApi;

		public ReleaseRecommendService(IFluentApi<Api.Schema.ReleaseEndpoint.ReleaseRecommend> recommendApi)
		{
			_recommendApi = recommendApi;
		}

		public Api.Schema.ReleaseEndpoint.ReleaseRecommend Get(ReleaseRecommendRequest request)
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