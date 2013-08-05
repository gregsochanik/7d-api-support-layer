using System;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Schema.ReleaseEndpoint;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.TestData
{
	public static class TestRelease
	{
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
	}
}