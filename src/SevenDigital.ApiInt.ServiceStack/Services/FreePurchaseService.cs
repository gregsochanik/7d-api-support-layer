using System.Linq;
using ServiceStack.ServiceInterface;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.Purchasing;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class FreePurchaseService : Service
	{
		private readonly IPurchaseItemMapper _mapper;
		private readonly IItemBuyer _itemBuyer;

		public FreePurchaseService(IPurchaseItemMapper mapper, IItemBuyer itemBuyer)
		{
			_mapper = mapper;
			_itemBuyer = itemBuyer;
		}

		public FreePurchaseResponse Get(FreePurchaseRequest request)
		{
			var accessToken = this.TryGetOAuthAccessToken();
			var lockerReleases = _itemBuyer.BuyItem(request, accessToken).ToList();

			return new FreePurchaseResponse
			{
				Item = _mapper.Map(request, lockerReleases),
				OriginalRequest = request,
				Status = new PurchaseStatus(true, "OK", lockerReleases)
			};
		}
	}
}