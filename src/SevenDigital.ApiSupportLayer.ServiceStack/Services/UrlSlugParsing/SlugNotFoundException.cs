using System;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.UrlSlugParsing
{
	public class SlugNotFoundException : Exception
	{
		public SlugNotFoundException(string slug) : base(string.Format("Slug {0} was not found", slug))
		{}
	}
}