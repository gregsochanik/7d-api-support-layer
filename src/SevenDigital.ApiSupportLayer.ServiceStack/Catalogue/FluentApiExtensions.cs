using System.Collections.Generic;
using System.Linq;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Catalogue
{
	public static class FluentApiExtensions
	{
		public static IEnumerable<IFluentApi<T>> GetApiCallsForAllCountries<T>(this IFluentApi<T> seedFluentApi)
		{
			return CatalogueHelper.CountriesToCheckInCatalogue.Select(country => seedFluentApi.WithParameter("country", country));
		}

		public static T LoopThroughCountriesUntil200<T>(this IFluentApi<T> seedFluentApi)
		{
			var apiCallsForAllCountries = seedFluentApi.GetApiCallsForAllCountries();
			ApiException exception = null;
			foreach (var apiCallsForAllCountry in apiCallsForAllCountries)
			{
				try
				{
					return apiCallsForAllCountry.Please();
				}
				catch (ApiException ex)
				{
					exception = ex;
				}
			}
			throw exception;
		}	
	}
}