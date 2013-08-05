namespace SevenDigital.ApiInt.Model
{
	public class CardBasketPurchaseRequest : ItemRequest
	{
		public int CardId { get; set; }
		public decimal Price { get; set; }
		public bool IsDefault { get; set; }
	}
}