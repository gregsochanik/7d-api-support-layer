using SevenDigital.Api.Schema.Premium;
using SevenDigital.Api.Schema.Premium.User;

namespace SevenDigital.ApiInt.User
{
	public interface IUserApi
	{
		bool CheckUserExists(string emailAddress);
		UserSignup Create(string emailAddress, string password);
	}
}