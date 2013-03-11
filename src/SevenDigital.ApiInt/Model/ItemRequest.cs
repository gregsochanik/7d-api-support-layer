using System.Runtime.Serialization;

namespace SevenDigital.ApiInt.Model
{
	[DataContract]
	public class ItemRequest: IHasId, IHasCountryCode
	{
		private string _countryCode = "GB";

		[DataMember]
		public string CountryCode
		{
			get { return _countryCode; }
			set { _countryCode = value; }
		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public PurchaseType Type { get; set; }

		[DataMember]
		public int PartnerId { get; set; }
	}
}