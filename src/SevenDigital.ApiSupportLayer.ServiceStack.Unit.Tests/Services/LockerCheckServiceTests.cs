using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.ApiSupportLayer.Locker;
using SevenDigital.ApiSupportLayer.Mapping;
using SevenDigital.ApiSupportLayer.Model;
using SevenDigital.ApiSupportLayer.ServiceStack.Model;
using SevenDigital.ApiSupportLayer.ServiceStack.Services;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Services
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