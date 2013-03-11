using System.Web;
using SevenDigital.Api.Schema.Premium.User;
using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiInt.User
{
	public class UserApi : IUserApi
	{
		private readonly IFluentApi<Users> _usersApi;
		private readonly IFluentApi<UserSignup> _userSignupApi;

		public UserApi(IFluentApi<Users> usersApi, IFluentApi<UserSignup> userSignupApi)
		{
			_usersApi = usersApi;
			_userSignupApi = userSignupApi;
		}

		public bool CheckUserExists(string emailAddress)
		{
			var usersFound = _usersApi.WithParameter("emailAddress", emailAddress).Please();
			return usersFound.UserList.Count > 0;
		}

		public UserSignup Create(string emailAddress, string password)
		{
			var userSignup = _userSignupApi.WithParameter("emailAddress", HttpUtility.UrlEncode(emailAddress)).WithParameter("password", HttpUtility.UrlEncode(password)).Please();
			return userSignup;
		} 
	}
}