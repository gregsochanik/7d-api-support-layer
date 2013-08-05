using System;
using SevenDigital.Api.Schema.OAuth;

namespace SevenDigital.ApiSupportLayer.Locker
{
	public interface ILockerDateChecker
	{
		DateTime GetLatestPurchaseDateOfLiveLocker(OAuthAccessToken accessToken);
	}
}