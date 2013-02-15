using System;
using System.Collections.Generic;
using System.Linq;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Extensions;

namespace SevenDigital.ApiInt.Locker
{
	public class LockerDateChecker : ILockerDateChecker
	{
		private readonly IFluentApi<Api.Schema.LockerEndpoint.Locker> _apiWrapper;

		public LockerDateChecker(IFluentApi<Api.Schema.LockerEndpoint.Locker> apiWrapper)
		{
			_apiWrapper = apiWrapper;
		}

		public DateTime GetLatestPurchaseDateOfLiveLocker(OAuthAccessToken accessToken)
		{
			var oneLockerRow = _apiWrapper.ForUser(accessToken.Token, accessToken.Secret)
			                              .WithPageNumber(1)
			                              .WithPageSize(1)
			                              .Sort(LockerSortColumn.PurchaseDate, SortOrder.Descending)
			                              .Please();

			if (!oneLockerRow.Response.LockerReleases.Any())
				return DateTime.MinValue;

			return GetLatestPurchaseDate(oneLockerRow.Response.LockerReleases);
		}


		private static DateTime GetLatestPurchaseDate(IEnumerable<LockerRelease> lockerReleases)
		{
			return lockerReleases.First()
			                     .LockerTracks.OrderByDescending(x => x.PurchaseDate).First()
			                     .PurchaseDate;
		}
	}
}