using System.Linq;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.Territories;
using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.Shops
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
				shopUrl.Description = enumerable.Description;
			}
			else if (ShopUrlConstants.GenericEuroCountryCodes().Contains(shopUrl.CountryCode))
			{
				shopUrl.DomainName = ShopUrlConstants.GENERIC_EURO_URL;
				shopUrl.Description = "European";
			}
			else
			{
				shopUrl.DomainName = "xw.7digital.com";
				shopUrl.Description = "Worldwide";
			}

			var urlPath = string.IsNullOrEmpty(shopUrl.UrlPath) ? string.Empty : "/" + shopUrl.UrlPath;

			return new HttpResult
			{
				Location = shopUrl.DomainName + urlPath,
				StatusCode = HttpStatusCode.Redirect,
				Response = shopUrl.Description
			};
		}
	}
}