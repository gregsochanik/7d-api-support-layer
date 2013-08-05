using ServiceStack.ServiceInterface;

namespace SevenDigital.ApiInt.ServiceStack.Model
{
	[Authenticate]
	public class CardRequest
	{
		public int Id { get; set; }
	}
}