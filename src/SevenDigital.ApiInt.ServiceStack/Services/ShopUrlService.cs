using System.Linq;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.Territories;
using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class ShopUrlService : Service
	{
		private readonly IFluentApi<Countries> _countryApi;

		public ShopUrlService(IFluentApi<Countries> countryApi)
		{
			_countryApi = countryApi;
		}

		public HttpResult Get(ShopUrl shopUrl)
		{
			var countries = _countryApi.Please();

			var enumerable = countries.CountryItems.FirstOrDefault(x => x.Code == shopUrl.CountryCode);

			if (enumerable != null)
			{
				shopUrl.DomainName = enumerable.Url;
			}
			else if (ShopUrlConstants.GenericEuroCountryCodes().Contains(shopUrl.CountryCode))
			{
				shopUrl.DomainName = ShopUrlConstants.GENERIC_EURO_URL;
			}
			else
			{
				shopUrl.DomainName = "xw.7digital.com";
			}

			return new HttpResult
			{
				Location = shopUrl.DomainName,
				StatusCode = HttpStatusCode.Redirect
			};
		}
	}
}