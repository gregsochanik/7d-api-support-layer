using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.ApiInt.Locker;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Services;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class LockerCheckServiceTests
	{
		[Test]
		public void Happy_path_release()
		{
			var purchaseItemMapper = MockRepository.GenerateStub<IPurchaseItemMapper>();

			var lockerBrowser = MockRepository.GenerateStub<ILockerBrowser>();
			lockerBrowser.Stub(x => x.GetLockerItem(null, null)).IgnoreArguments().Return(new LockerResponse()
			{
				LockerReleases = new List<LockerRelease>()
			});


			var lockerCheckService = new LockerCheckService(lockerBrowser, purchaseItemMapper)
				                         {
					                         RequestContext = ContextHelper.LoggedInContext()
				                         };
			var o = lockerCheckService.Get(new LockerCheckRequest() {Type = PurchaseType.release});
		}
	}
}