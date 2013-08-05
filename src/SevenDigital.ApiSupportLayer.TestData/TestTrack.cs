using SevenDigital.Api.Schema.Pricing;
using SevenDigital.Api.Schema.TrackEndpoint;

namespace SevenDigital.ApiInt.TestData
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

		public static Track EverybodysChanging
		{
			get
			{
				return new Track
				{
					Artist = TestArtist.Keane,
					Duration = 216,
					ExplicitContent = false,
					Id = 1532,
					Image = "http://cdn.7static.com/static/img/sleeveart/00/002/653/0000265341_50.jpg",
					Isrc = "GBAAN0400092",
					Price = new Price(),
					Release = TestRelease.HopesAndFears,
					Title = "Everybody's Changing",
					TrackNumber = 4,
					Type = TrackType.track,
					Url = "http://www.7digital.com/artist/keane/release/hopes-and-fears/?partner=712&amp;h=01",
					Version = ""
				};
			}
		}

		public static Track VariousArtistsTrack
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
					Release = TestRelease.VariousArtistsCompilation,
					Title = "Sun It Rises",
					TrackNumber = 1,
					Type = TrackType.track,
					Url = "http://www.7digital.com/artist/fleet-foxes/release/fleet-foxes/?partner=712&amp;h=01",
					Version = ""
				};
			}
		}

		public static Track NonAudioTrack
		{
			get
			{
				return new Track
				{
					Artist = TestArtist.FleetFoxes,
					Duration = 0,
					ExplicitContent = false,
					Id = 2854213,
					Image = "http://cdn.7static.com/static/img/sleeveart/00/002/653/0000265341_50.jpg",
					Isrc = "GBBRP0816701",
					Price = new Price(),
					Release = TestRelease.FleetFoxes,
					Title = "Sun It Rises - the book",
					TrackNumber = 2,
					Type = TrackType.track,
					Url = "http://www.7digital.com/artist/fleet-foxes/release/fleet-foxes/?partner=712&amp;h=01",
					Version = "Read"
				};
			}
		}

		public static Track BundleTrack
		{
			get
			{
				return new Track
				{
					Artist = TestArtist.FleetFoxes,
					Duration = 0,
					ExplicitContent = false,
					Id = 2854213,
					Image = "http://cdn.7static.com/static/img/sleeveart/00/002/653/0000265341_50.jpg",
					Isrc = "GBBRP0816701",
					Price = new Price{Value="", FormattedPrice = "N/A"},
					Release = TestRelease.FleetFoxes,
					Title = "Sun It Rises - the book",
					TrackNumber = 2,
					Type = TrackType.track,
					Url = "http://www.7digital.com/artist/fleet-foxes/release/fleet-foxes/?partner=712&amp;h=01",
					Version = "Read"
				};
			}
		}
	}
}