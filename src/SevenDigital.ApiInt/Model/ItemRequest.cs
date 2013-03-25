namespace SevenDigital.ApiInt.Model
{
	public class ItemRequest: IHasId, IHasCountryCode
	{
		private string _countryCode = "GB";

		public string CountryCode
		{
			get { return _countryCode; }
			set { _countryCode = value; }
		}

		public int Id { get; set; }

		public int? ReleaseId { get; set; }

		public PurchaseType Type { get; set; }

		public int PartnerId { get; set; }
	}
}