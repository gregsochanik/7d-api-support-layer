using System.Runtime.Serialization;

namespace SevenDigital.ApiInt.Model
{
	[DataContract]
	public class PurchaseRequest 
	{
		[DataMember]
		public PurchaseType Type { get; set; }

		[DataMember]
		public int Id { get; set; }
	}
}