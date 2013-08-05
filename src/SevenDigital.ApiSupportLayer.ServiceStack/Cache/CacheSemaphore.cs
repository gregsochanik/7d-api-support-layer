using System.Collections.Generic;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Cache
{
	public static class CacheSemaphore
	{
		private static readonly object _synchlock = new object();
		private static volatile Dictionary<string, bool> _cacheLock;

		public static Dictionary<string, bool> Instance
		{
			get
			{
				if (_cacheLock == null)
				{
					lock (_synchlock)
					{
						if (_cacheLock == null)
						{
							_cacheLock = new Dictionary<string, bool>();
						}
					}
				}
				return _cacheLock;
			}
		}
	}
}