using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiSupportLayer.Catalogue
{
	public interface IFluentApiTriggers
	{
		T SingleRequest<T>(IFluentApi<T> fluentApi, string countryCode);
		T MultipleRequestBasedOnCountryCodeList<T>(IFluentApi<T> fluentApi);
	}
}