using SevenDigital.Api.Schema.Pricing;
using SevenDigital.Api.Schema.TrackEndpoint;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.TestData
{
	public static class TestTrack
	{
		public static Track SunItRises
		{
			get
			{
				return new Track
				{
					Artist = TestArtist.FleetFoxes,
					Duration = 191,
					ExplicitContent = false,
					Id = 2854214,
					Image = "http://cdn.7static.com/static/img/sleeveart/00/002/653/0000265341_50.jpg",
					Isrc = "GBBRP0816701",
					Price = new Price(),
					Release = TestRelease.FleetFoxes,
					Title = "Sun It Rises",
					TrackNumber = 1,
					Type = TrackType.track,
					Url = "http://www.7digital.com/artist/fleet-foxes/release/fleet-foxes/?partner=712&amp;h=01",
					Version = ""
				};
			}
		}
	}
}