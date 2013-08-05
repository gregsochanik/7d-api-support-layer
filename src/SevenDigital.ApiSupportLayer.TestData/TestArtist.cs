using Rhino.Mocks;
using SevenDigital.Api.Schema.ArtistEndpoint;
using SevenDigital.Api.Wrapper;

namespace SevenDigital.ApiSupportLayer.TestData
{
	public static class TestArtist
	{
		public static Artist FleetFoxes
		{
			get
			{
				return new Artist
				{
					AppearsAs = "Fleet Foxes",
					Id = 162919,
					Image = "http://cdn.7static.com/static/img/artistimages/00/001/629/0000162919_150.jpg",
					Name = "Fleet Foxes",
					SortName = "Fleet Foxes",
					Url = "http://www.7digital.com/artists/fleet-foxes/?partner=712"
				};
			}
		}

		public static Artist Keane
		{
			get
			{
				return new Artist
				{
					AppearsAs = "Keane",
					Id = 1,
					Image = "http://cdn.7static.com/static/img/artistimages/00/000/000/0000000001_150.jpg",
					Name = "Keane",
					SortName = "Keane",
					Url = "http://www.7digital.com/artists/keane/?partner=712"
				};
			}
		}

		public static Artist MinistryOfSound
		{
			get
			{
				return new Artist
				{
					AppearsAs = "Ministry of Sound",
					Id = 141591,
					Image = "http://cdn.7static.com/static/img/artistimages/00/000/000/0000000001_150.jpg",
					Name = "Ministry of Sound",
					SortName = "Ministry of Sound",
					Url = "http://www.7digital.com/artists/ministry-of-sound/?partner=712"
				};
			}
		}

		public static IFluentApi<Artist> GetStubbedArtistApi(Artist stubbedArtistToReturn)
		{
			var apiLocker = MockRepository.GenerateStub<IFluentApi<Artist>>();
			apiLocker.Stub(x => x.WithArtistId(0)).IgnoreArguments().Return(apiLocker);
			apiLocker.Stub(x => x.Please()).Return(stubbedArtistToReturn);

			return apiLocker;
		}
	}
}