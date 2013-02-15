using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Purchasing
{
	public interface IItemBuyer
	{
		IEnumerable<LockerRelease> BuyItem(ItemRequest request, OAuthAccessToken accessToken);
	}
}