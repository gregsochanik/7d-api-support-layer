using System;

namespace SevenDigital.ApiSupportLayer.Authentication
{
	public class LoginInvalidException : Exception
	{
		public LoginInvalidException() : base("Login invalid")
		{}

		public LoginInvalidException(string message) : base(message)
		{}
	}
}