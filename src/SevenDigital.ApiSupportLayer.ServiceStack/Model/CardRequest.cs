using ServiceStack.ServiceInterface;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Model
{
	[Authenticate]
	public class CardRequest
	{
		public int Id { get; set; }
	}
}