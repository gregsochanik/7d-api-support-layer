using NUnit.Framework;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Services
{
	[TestFixture]
	public class ItemRequestExtensionTests
	{
		[Test]
		public void HasReleaseId_is_false_by_default()
		{
			var itemRequest = new ItemRequest();
			Assert.That(itemRequest.HasReleaseId(), Is.False);
		}

		[Test]
		public void HasReleaseId_returns_false_if_0()
		{
			var itemRequest = new ItemRequest{ReleaseId = 0};
			Assert.That(itemRequest.HasReleaseId(), Is.False);
		}

		[Test]
		public void HasReleaseId_returns_false_if_Purchasetype_release()
		{
			var itemRequest = new ItemRequest { ReleaseId = 12, Type=PurchaseType.release };
			Assert.That(itemRequest.HasReleaseId(), Is.False);
		}

		[Test]
		public void HasReleaseId_returns_true_if_is_track_and_releaseId_specified_gt_0()
		{
			var itemRequest = new ItemRequest { ReleaseId = 12, Type = PurchaseType.track};
			Assert.That(itemRequest.HasReleaseId());
		}
	}
}