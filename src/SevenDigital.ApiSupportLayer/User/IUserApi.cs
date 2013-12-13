using SevenDigital.Api.Schema.Premium.User;

namespace SevenDigital.ApiSupportLayer.User
{
	public interface IUserApi
	{
		bool CheckUserExists(string emailAddress);
		UserSignup Create(string emailAddress, string password);
		UserSignup Create(string emailAddress, string password, string affiliatePartner);
	}
}