using SevenDigital.Api.Schema.ArtistEndpoint;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.TestData
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
	}
}