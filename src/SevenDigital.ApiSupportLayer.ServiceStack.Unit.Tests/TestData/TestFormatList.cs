using System.Collections.Generic;
using SevenDigital.Api.Schema.Media;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.TestData
{
	public static class TestFormatList
	{
		public static FormatList OneMp3
		{
			get { return new FormatList {  AvailableDrmFree = true, Formats = new List<Format>{TestFormat.Mp3} }; }
		}
	}
}