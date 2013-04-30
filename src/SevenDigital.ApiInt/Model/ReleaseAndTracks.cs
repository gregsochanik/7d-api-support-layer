using System.Collections.Generic;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;

namespace SevenDigital.ApiInt.Model
{
	public class ReleaseAndTracks
	{
		public Release Release { get; set; }

		public List<Track> Tracks { get; set; }

		public PurchaseType Type { get; set; }

		public int TrackCount { get; set; }
	}
}