namespace SevenDigital.ApiSupportLayer.Restrictions
{
	public class Restriction
	{
		private readonly RestrictionType _type;
		private readonly string _message;

		public Restriction(RestrictionType type, string message)
		{
			_type = type;
			_message = message;
		}

		public RestrictionType Type
		{
			get { return _type; }
		}

		public string Message 
		{
			get { return _message; }
		}
	}
}