namespace SevenDigital.ApiSupportLayer.Model
{
	public class CardPurchaseRequest : ItemRequest
	{
		public int CardId { get; set; }
		public decimal Price { get; set; }
		public bool IsDefault { get; set; }
	}
}