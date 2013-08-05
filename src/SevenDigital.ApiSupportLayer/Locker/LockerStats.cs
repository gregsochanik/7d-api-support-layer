namespace SevenDigital.ApiInt.Locker
{
	public class LockerStats
	{
		public LockerStats(int totalItems)
		{
			TotalItems = totalItems;
		}
		public int TotalItems { get; private set; }

		public bool Equals(LockerStats other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return other.TotalItems == TotalItems;
		}

		public override int GetHashCode()
		{
			return TotalItems;
		}

		public override string ToString()
		{
			return TotalItems.ToString();
		}
	}
}