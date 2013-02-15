using System;
using ServiceStack.Logging;

namespace SevenDigital.ApiInt.Authentication
{
	public class LoginInvalidException : Exception
	{
		private readonly ILog _logger = LogManager.GetLogger("LoginService");
		public LoginInvalidException(string body) : base("Login invalid")
		{
			_logger.Info(body);
		}
	}
}