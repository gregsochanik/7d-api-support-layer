using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Extensions;

namespace SevenDigital.ApiInt.Locker
{
	public class LockerRetrieval : ILockerRetrieval
	{
		private const int MAX_RECORDS_TO_PAGE_FROM_LOCKER_API = 500;
		private readonly IFluentApi<Api.Schema.LockerEndpoint.Locker> _apiWrapper;

		public LockerRetrieval(IFluentApi<Api.Schema.LockerEndpoint.Locker> apiWrapper)
		{
			_apiWrapper = apiWrapper;
		}

		public IEnumerable<LockerRelease> GetLockerReleases(OAuthAccessToken accessToken, LockerStats lockerStats)
		{
			if (lockerStats.TotalItems < 1)
				return new LockerRelease[] { };

			var fullLockerReleases = new LockerRelease[lockerStats.TotalItems];
			var numberOfLockerPages = lockerStats.TotalItems / MAX_RECORDS_TO_PAGE_FROM_LOCKER_API;

			for (var page = 1; page <= numberOfLockerPages; page++) // NOTE: Array index insert method to enable use of Parallel.For loading
			{
				InsertSinglePageOfLocker(fullLockerReleases, accessToken, page);
			}

			var finalLockerPage = numberOfLockerPages + 1;
			InsertSinglePageOfLocker(fullLockerReleases, accessToken, finalLockerPage);

			return fullLockerReleases;
		}

		public LockerStats GetLockerStats(OAuthAccessToken accessToken)
		{
			var lockerTotalItems = _apiWrapper.ForUser(accessToken.Token, accessToken.Secret)
			                                  .WithPageNumber(1)
			                                  .WithPageSize(1)
			                                  .Please()
			                                  .Response.TotalItems;

			return new LockerStats(lockerTotalItems);
		}

		private void InsertSinglePageOfLocker(IList<LockerRelease> fullLockerReleases, OAuthAccessToken token, int currentPage)
		{
			var pageOfReleases = CallLockerApi(token, MAX_RECORDS_TO_PAGE_FROM_LOCKER_API, currentPage);
			var startIndexOfCurrentPage = MAX_RECORDS_TO_PAGE_FROM_LOCKER_API * (currentPage - 1);

			AddRangeAt(startIndexOfCurrentPage, fullLockerReleases, pageOfReleases);
		}

		private static void AddRangeAt(int startIndex, IList<LockerRelease> fullLockerReleases, IList<LockerRelease> releasesToAdd)
		{
			for (var currentPagedIndex = 0; currentPagedIndex < releasesToAdd.Count; currentPagedIndex++)
			{
				var currentGlobalIndex = startIndex + currentPagedIndex;
				fullLockerReleases[currentGlobalIndex] = releasesToAdd[currentPagedIndex];
			}
		}

		private List<LockerRelease> CallLockerApi(OAuthAccessToken accessToken, int pageSize, int pageNumber)
		{
			return _apiWrapper.ForUser(accessToken.Token, accessToken.Secret)
			                  .Sort(LockerSortColumn.PurchaseDate, SortOrder.Descending)
			                  .WithPageNumber(pageNumber)
			                  .WithPageSize(pageSize)
			                  .WithParameter("imagesize", "100")
			                  .Please()
			                  .Response.LockerReleases;
		}
	}
}