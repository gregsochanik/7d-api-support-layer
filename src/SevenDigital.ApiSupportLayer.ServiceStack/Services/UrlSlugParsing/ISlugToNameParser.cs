using System;

namespace SevenDigital.ApiInt.ServiceStack.Services.UrlSlugParsing
{
	public interface ISlugToNameParser
	{
		string ArtistFromUri(Uri uri);
		string ReleaseFromUri(Uri uri);
	}
}