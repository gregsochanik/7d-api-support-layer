namespace SevenDigital.ApiInt.Model
{
	public class TrackSearchRequest
	{
		public string Query { get; set; }
		public string CountryCode { get; set; }
		public int PageSize { get; set; }
	}
}