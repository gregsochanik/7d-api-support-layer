using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.Mapping
{
	public interface IPurchaseItemMapper
	{
		PurchasedItem Map(ItemRequest request, IEnumerable<LockerRelease> lockerReleases);
	}
}