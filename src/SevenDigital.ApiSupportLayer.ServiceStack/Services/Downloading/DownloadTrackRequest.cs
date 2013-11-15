using ServiceStack.ServiceInterface;
using SevenDigital.ApiSupportLayer.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.Downloading
{
	[Authenticate]
	public class DownloadTrackRequest : ItemRequest
	{
		public int FormatId { get; set; }
	}
}