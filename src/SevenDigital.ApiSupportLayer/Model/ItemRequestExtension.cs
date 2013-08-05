namespace SevenDigital.ApiSupportLayer.Model
{
	public static class ItemRequestExtension
	{
		public static bool HasReleaseId(this ItemRequest request)
		{
			return request.ReleaseId.HasValue && request.ReleaseId > 0 && request.Type == PurchaseType.track;
		}
	}
}