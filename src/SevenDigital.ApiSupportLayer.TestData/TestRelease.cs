using System;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Schema.ReleaseEndpoint;

namespace SevenDigital.ApiInt.TestData
{
	public static class TestRelease
	{
		public static Release Get(int id, string title)
		{
			var tempRelease = FleetFoxes;
			tempRelease.Id = id;
			tempRelease.Title = title;

			return tempRelease;
		}

		public static Release FleetFoxes
		{
			get
			{
				return new Release
				       	{
				       		AddedDate = DateTime.Now,
				       		Artist = TestArtist.FleetFoxes,
				       		Barcode = "05033197507699",
				       		ExplicitContent = false,
				       		Formats = TestFormatList.OneMp3,
				       		Id = 265341,
				       		Image = "http://cdn.7static.com/static/img/sleeveart/00/002/653/0000265341_50.jpg",
				       		Label = new Label {Id = 14824, Name = "Universal Music s.r.o."},
				       		Licensor = null,
				       		Price = TestPrice.Uk799,
				       		ReleaseDate = new DateTime(2008, 06, 09),
				       		Title = "Fleet Foxes",
				       		Type = ReleaseType.Album,
				       		Url = "http://www.7digital.com/artists/fleet-foxes/fleet-foxes/?partner=712",
				       		Version = "",
				       		Year = "2007"
				       	};
			}
		}

		public static Release VariousArtistsCompilation
		{
			get
			{
				return new Release
				{
					AddedDate = DateTime.Now,
					Artist = TestArtist.MinistryOfSound,
					Barcode = "05033197507699",
					ExplicitContent = false,
					Formats = TestFormatList.OneMp3,
					Id = 265341,
					Image = "http://cdn.7static.com/static/img/sleeveart/00/002/653/0000265341_50.jpg",
					Label = new Label { Id = 14824, Name = "Universal Music s.r.o." },
					Licensor = null,
					Price = TestPrice.Uk799,
					ReleaseDate = new DateTime(2008, 06, 09),
					Title = "Fleet Foxes",
					Type = ReleaseType.Album,
					Url = "http://www.7digital.com/artists/fleet-foxes/fleet-foxes/?partner=712",
					Version = "",
					Year = "2007"
				};
			}
		}

		public static Release HopesAndFears
		{
			get
			{
				return new Release
				{
					AddedDate = DateTime.Now,
					Artist = TestArtist.Keane,
					Barcode = "00600753225530",
					ExplicitContent = false,
					Formats = TestFormatList.OneMp3,
					Id = 625642,
					Image = "http://cdn.7static.com/static/img/sleeveart/00/006/256/0000625642_50.jpg",
					Label = new Label() { Id = 670, Name = "Interscope" },
					Licensor = null,
					Price = TestPrice.Uk799,
					ReleaseDate = new DateTime(2009, 11, 09),
					Title = "Hopes and Fears",
					Type = ReleaseType.Album,
					Url = "http://www.7digital.com/artists/keane/hopes-and-fears-3/?partner=712",
					Version = "Deluxe Edition",
					Year = "2009"
				};
			}
		}

		public static Release FutureRelease
		{
			get
			{
				return new Release
				{
					AddedDate = DateTime.Now,
					Artist = TestArtist.Keane,
					Barcode = "00600753225530",
					ExplicitContent = false,
					Formats = TestFormatList.OneMp3,
					Id = 625642,
					Image = "http://cdn.7static.com/static/img/sleeveart/00/006/256/0000625642_50.jpg",
					Label = new Label() { Id = 670, Name = "Interscope" },
					Licensor = null,
					Price = TestPrice.Uk799,
					ReleaseDate = DateTime.Now.AddDays(1),
					Title = "Future Hopes and Fears",
					Type = ReleaseType.Album,
					Url = "http://www.7digital.com/artists/keane/hopes-and-fears-3/?partner=712",
					Version = "Deluxe Edition",
					Year = "2009"
				};
			}
		}
	}
}