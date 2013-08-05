using SevenDigital.Api.Schema.Media;

namespace SevenDigital.ApiInt.TestData
{
	public static class TestFormat
	{
		public static Format Mp3
		{
			get { return new Format {BitRate = "320", DrmFree = true, FileFormat = "MP3", Id=17 }; }
		}

		public static Format M4A
		{
			get { return new Format { BitRate = "320", DrmFree = true, FileFormat = "M4A", Id = 33 }; }
		}

		public static Format Pdf
		{
			get { return new Format { BitRate = "", DrmFree = true, FileFormat = "PDF", Id=10 }; }
		}
	}
}