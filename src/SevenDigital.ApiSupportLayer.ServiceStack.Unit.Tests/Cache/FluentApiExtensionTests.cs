using System.Linq;
using System.Net;
using NUnit.Framework;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Wrapper.Exceptions;
using Rhino.Mocks;
using SevenDigital.ApiSupportLayer.Catalogue;
using SevenDigital.ApiSupportLayer.ServiceStack.Catalogue;
using SevenDigital.ApiSupportLayer.TestData.StubApiWrapper;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Cache
{
	[TestFixture]
	public class FluentApiExtensionTests
	{
		private const int NUMBER_OF_COUNTRIES_SPECIFIED_TO_CHECK = 4;

		[Test]
		public void returns_a_list_of_fluent_apis_for_all_countries()
		{
			var stubbedApi = ApiWrapper.StubbedTrackApi();

			var apiCallsForAllCountries = stubbedApi.GetApiCallsForAllCountries().ToList();
			Assert.That(apiCallsForAllCountries.Count, Is.EqualTo(NUMBER_OF_COUNTRIES_SPECIFIED_TO_CHECK));
		}

		[Test]
		public void Gives_back_the_correct_Track_if_the_first_2_calls_error()
		{
			var stubbedApi = ApiWrapper.StubbedTrackApi();

			stubbedApi.Stub(x => x.Please()).Throw(new ApiWebException("", "", new WebException())).Repeat.Times(2);
			stubbedApi.Stub(x => x.Please()).Return(new Track{Title="Blah"}).Repeat.Once();

			var loopUntil200 = stubbedApi.LoopThroughCountriesUntil200();

			Assert.That(loopUntil200.Title, Is.EqualTo("Blah"));
		}

		[Test]
		public void Throws_last_api_error_if_no_data_returned_for_any_countries()
		{
			var stubbedApi = ApiWrapper.StubbedTrackApi();

			var apiWebException = new ApiWebException("", "", new WebException());
			stubbedApi.Stub(x => x.Please()).Throw(apiWebException).Repeat.Times(5);

			Assert.Throws<ApiWebException>(() => stubbedApi.LoopThroughCountriesUntil200());
		}


	}
}