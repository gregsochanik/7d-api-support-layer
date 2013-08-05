namespace SevenDigital.ApiSupportLayer.MediaDelivery
{
	public class StreamingSettings
	{
		private const int AAC_160_FORMAT_ID = 56;
		private const int MP3_128_FORMAT_ID = 26;

		public const string LOCKER_STREAMING_URL = "http://stream.svc.7digital.net/stream/locker";
		public const string SUBS_STREAMING_URL = "http://stream.svc.7digital.net/stream/subscription";

		private static readonly MimeTypeFormatCombination _aac160 = new MimeTypeFormatCombination(AAC_160_FORMAT_ID, "audio/aac");
		private static readonly MimeTypeFormatCombination _mp3128 = new MimeTypeFormatCombination(MP3_128_FORMAT_ID, "audio/mp3");

		public static readonly MimeTypeFormatCombination CurrentStreamType = _aac160;
	}

	public class DownloadSettings
	{
		private const int MP3_320_FORMAT_ID = 17;

		public const string DOWNLOAD_RELEASE_URL = "http://media.geo.7digital.com/media/user/download/release";
		public const string DOWNLOAD_TRACK_URL = "http://media.geo.7digital.com/media/user/downloadtrack";

		private static readonly MimeTypeFormatCombination _mp3320 = new MimeTypeFormatCombination(MP3_320_FORMAT_ID, "audio/mp3");

		public static readonly MimeTypeFormatCombination CurrentDownloadType = _mp3320;
	}
}