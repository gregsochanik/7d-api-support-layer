using System;
using System.Collections.Generic;
using System.Linq;
using SevenDigital.Api.Wrapper;
using SevenDigital.Api.Wrapper.Exceptions;

namespace SevenDigital.ApiInt.ServiceStack.Catalogue
{
	public static class FluentApiExtensions
	{
		private static readonly string[] _countriesToCheckInCatalogue = new[] { "GB", "US", "DE", "FR" };

		public static IEnumerable<IFluentApi<T>> GetApiCallsForAllCountries<T>(this IFluentApi<T> seed)
		{
			return _countriesToCheckInCatalogue.Select(country => seed.WithParameter("country", country));
		}

		public static T LoopUntil200<T>(this IFluentApi<T> seed)
		{
			var apiCallsForAllCountries = seed.GetApiCallsForAllCountries();
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