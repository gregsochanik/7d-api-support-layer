using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiInt.ServiceStack.Catalogue
{
	public class FluentApiTriggers : IFluentApiTriggers
	{
		public T SingleRequest<T>(IFluentApi<T> fluentApi)
		{
			return fluentApi.Please();
		}

		public T MultipleRequestBasedOnCountryCodeList<T>(IFluentApi<T> fluentApi)
		{
			return fluentApi.LoopThroughCountriesUntil200();
		}
	}
}