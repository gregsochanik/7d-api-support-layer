using System;
using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiInt.Locker
{
	public interface ILockerDateChecker
	{
		DateTime GetLatestPurchaseDateOfLiveLocker(OAuthAccessToken accessToken);
	}
}