using SevenDigital.Api.Schema.Media;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.TestData
{
	public static class TestFormat
	{
		public static Format Mp3
		{
			get { return new Format { BitRate = "320", DrmFree = true, FileFormat = "MP3", Id = 17 }; }
		}
	}
}