using System;
using ServiceStack.ServiceInterface;

namespace SevenDigital.ApiInt.ServiceStack.Model
{
	[Authenticate]
	public class AddCardRequest : CardRequest
	{
		public DateTime ExpiryDate { get; set; }
		public string HolderName { get; set; }
		public string IssueNumber { get; set; }
		public string Number { get; set; }
		public string PostCode { get; set; }
		public DateTime StartDate { get; set; }
		public string TwoLetterISORegionName { get; set; }
		public string Type { get; set; }
		public string VerificationCode { get; set; }
	}
}