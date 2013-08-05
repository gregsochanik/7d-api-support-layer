using System;

namespace SevenDigital.ApiInt.Authentication
{
	public class LoginInvalidException : Exception
	{
		public LoginInvalidException() : base("Login invalid")
		{}

		public LoginInvalidException(string message) : base(message)
		{}
	}
}