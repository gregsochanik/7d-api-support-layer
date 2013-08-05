using SevenDigital.Api.Schema.Pricing;

namespace SevenDigital.ApiInt.ServiceStack.Unit.Tests.TestData
{
	public static class TestPrice
	{
		public static Price Uk799
		{
			get { return new Price { Currency = new Currency { Code = "GBP" }, FormattedPrice = "£7.99", FormattedRrp = "£7.99", IsOnSale = false, Rrp = "7.99", Value = "7.99" }; }
		}
	}
}