using System;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.UrlSlugParsing
{
	public class SlugToNameParser : ISlugToNameParser
	{
		public string ArtistFromUri(Uri uri)
		{
			var segments = uri.Segments;

			var artistPos = Array.FindIndex(segments, x => x == "artist/");
			if (artistPos < 0)
			{
				throw new SlugParseException();
			}

			return segments[(artistPos + 1)].Replace("/", "");
		}

		public string ReleaseFromUri(Uri uri)
		{
			var segments = uri.Segments;

			var artistPos = Array.FindIndex(segments, x => x == "release/");
			if (artistPos < 0)
			{
				throw new SlugParseException();
			}

			return segments[(artistPos + 1)].Replace("/", "");
		}
	}
}