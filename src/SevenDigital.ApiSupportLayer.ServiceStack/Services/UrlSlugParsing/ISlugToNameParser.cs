using System;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.UrlSlugParsing
{
	public interface ISlugToNameParser
	{
		string ArtistFromUri(Uri uri);
		string ReleaseFromUri(Uri uri);
	}
}