using System;

namespace SevenDigital.ApiSupportLayer.Exceptions
{
	public class InvalidBasketIdException : Exception
	{
		public InvalidBasketIdException(string givenId, Exception innerException) 
			: base(string.Format("BasketId {0} is invalid", givenId), innerException)
		{}
	}
}