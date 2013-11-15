using System;

namespace SevenDigital.ApiSupportLayer.Cache
{
	public interface ICacheLock
	{
		bool IsLocked(string lockOn);
		void PerformLockedTask(string lockOn, Action task);
	}
}