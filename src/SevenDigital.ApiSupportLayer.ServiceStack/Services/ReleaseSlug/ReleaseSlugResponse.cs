using SevenDigital.Api.Schema.ReleaseEndpoint;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.ReleaseSlug
{
	public class ReleaseSlugResponse
	{
		public ReleaseSlugRequest OriginalRequest { get; set; } 
		public Release Release { get; set; }
	}
}