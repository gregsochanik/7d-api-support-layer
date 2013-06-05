using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Mapping
{
	public interface IPurchaseItemMapper
	{
		PurchasedItem Map(ItemRequest request, IEnumerable<LockerRelease> lockerReleases);
	}
}