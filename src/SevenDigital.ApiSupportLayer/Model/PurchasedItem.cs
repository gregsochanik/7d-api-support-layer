using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;

namespace SevenDigital.ApiSupportLayer.Model
{
	public class PurchasedItem
	{
		public PurchasedItem()
		{
			DownloadUrls = new List<DownloadUrl>();
			Tracks = new List<Track>();
			Title = string.Empty;
		}

		public int Id { get; set; }
		public string Title { get; set; }
		public List<DownloadUrl> DownloadUrls { get; set; } 
		public List<Track> Tracks { get; set; }
	}
}