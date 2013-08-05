using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.OAuth;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Purchasing
{
	public interface IItemBuyer
	{
		IEnumerable<LockerRelease> BuyItem(ItemRequest request, OAuthAccessToken accessToken);
	}
}