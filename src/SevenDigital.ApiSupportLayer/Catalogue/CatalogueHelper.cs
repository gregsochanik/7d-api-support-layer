using System.Collections.ObjectModel;

namespace SevenDigital.ApiSupportLayer.Catalogue
{
	public static class CatalogueHelper
	{
		private static readonly string[] _countriesToCheckInCatalogue = new[] { "GB", "US", "DE", "FR" };

		public static ReadOnlyCollection<string> CountriesToCheckInCatalogue
		{
			get
			{
				return new ReadOnlyCollection<string>(_countriesToCheckInCatalogue);
			}
		}
	}
}