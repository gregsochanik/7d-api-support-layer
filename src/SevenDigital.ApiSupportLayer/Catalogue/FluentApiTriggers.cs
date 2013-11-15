using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiSupportLayer.Catalogue
{
	public class FluentApiTriggers : IFluentApiTriggers
	{
		public T SingleRequest<T>(IFluentApi<T> fluentApi, string countryCode)
		{
			return fluentApi.WithParameter("country", countryCode).Please();
		}

		public T MultipleRequestBasedOnCountryCodeList<T>(IFluentApi<T> fluentApi)
		{
			return fluentApi.LoopThroughCountriesUntil200();
		}
	}
}