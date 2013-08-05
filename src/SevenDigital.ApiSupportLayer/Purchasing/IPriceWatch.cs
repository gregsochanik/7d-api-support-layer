using SevenDigital.ApiInt.Model;

namespace SevenDigital.ApiInt.Purchasing
{
	public interface IPriceWatch
	{
		decimal GetReleasePrice(string countryCode, int id);
		decimal GetTrackPrice(string countryCode, int id);
		decimal GetItemPrice(ItemRequest request);
	}
}