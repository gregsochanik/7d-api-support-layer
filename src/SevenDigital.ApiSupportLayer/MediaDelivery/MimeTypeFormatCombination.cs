namespace SevenDigital.ApiInt.MediaDelivery
{
	public struct MimeTypeFormatCombination
	{
		private readonly int _formatId;
		private readonly string _mimetype;

		public MimeTypeFormatCombination(int formatId, string mimetype)
		{
			_formatId = formatId;
			_mimetype = mimetype;
		}

		public int FormatId
		{
			get { return _formatId; }
		}
		public string MimeType
		{
			get { return _mimetype; }
		}
	}
}