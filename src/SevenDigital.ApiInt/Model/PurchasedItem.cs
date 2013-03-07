using System.Collections.Generic;
using SevenDigital.Api.Schema.Media;
using SevenDigital.Api.Schema.TrackEndpoint;

namespace SevenDigital.ApiInt.Model
{
	public class PurchasedItem
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public List<Format> AvailableFormats { get; set; } 
		public List<Track> Tracks { get; set; }
	}
}