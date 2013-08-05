using System.Collections.Generic;
using SevenDigital.Api.Schema.LockerEndpoint;

namespace SevenDigital.ApiSupportLayer.Model
{
	public class PurchaseStatus
	{
		public PurchaseStatus(bool isSuccess, string message, IEnumerable<LockerRelease> updatedLocker)
		{
			IsSuccess = isSuccess;
			Message = message;
			UpdatedLocker = updatedLocker;
		}

		public bool IsSuccess { get; set; }
		public string Message { get; set; }
		public IEnumerable<LockerRelease> UpdatedLocker { get; set; }
	}
}