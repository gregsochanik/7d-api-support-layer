using System.Collections.Generic;
using System.Runtime.Serialization;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;

namespace SevenDigital.ApiInt.Model
{
	[DataContract]
	public class ReleaseAndTracks
	{
		[DataMember]
		public Release Release { get; set; }

		[DataMember]
		public List<Track> Tracks { get; set; }

		[DataMember]
		public PurchaseType Type { get; set; }
	}
}