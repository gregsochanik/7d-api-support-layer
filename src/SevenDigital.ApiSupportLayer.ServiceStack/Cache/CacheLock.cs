using System;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Cache
{
	public class CacheLock : ICacheLock
	{
		public void PerformLockedTask(string lockOn, Action task)
		{
			if (!IsLocked(lockOn))
			{
				Lock(lockOn);
				try
				{
					task();
				}
				catch (Exception ex)
				{
					Unlock(lockOn);
				}
				finally
				{
					Unlock(lockOn);
				}
			}
			else
			{
			}
		}

		public bool IsLocked(string lockOn)
		{
			return CacheSemaphore.Instance.ContainsKey(lockOn)
			       && CacheSemaphore.Instance[lockOn];
		}

		private static void Lock(string key)
		{
			CacheSemaphore.Instance[key] = true;
		}

		private static void Unlock(string key)
		{
			CacheSemaphore.Instance[key] = false;
			CacheSemaphore.Instance.Remove(key);
		}
	}
}