using System.Collections.Generic;
using NUnit.Framework;
using SevenDigital.ApiSupportLayer.Cache;

namespace SevenDigital.ApiSupportLayer.Unit.Tests.Cache
{
	[TestFixture]
	public class CacheLockTests
	{
		[Test]
		public void Unlocked_as_default()
		{
			var cacheLock = new CacheLock();
			var isLocked = cacheLock.IsLocked("Test");
			Assert.That(isLocked, Is.False);
		}

		[Test]
		public void Locked_task_is_locked_for_duration_of_task()
		{
			var cacheLock = new CacheLock();
			cacheLock.PerformLockedTask("Test", () => Assert.That(cacheLock.IsLocked("Test")));
			Assert.That(cacheLock.IsLocked("Test"), Is.False);
		}
	}
}
