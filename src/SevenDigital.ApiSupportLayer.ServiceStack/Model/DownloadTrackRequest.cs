﻿using ServiceStack.ServiceInterface;
using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.ServiceStack.Model
{
	[Authenticate]
	public class DownloadTrackRequest : ItemRequest
	{
		public int FormatId { get; set; }
	}
}