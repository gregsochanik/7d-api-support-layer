using System;
using NUnit.Framework;
using SevenDigital.ApiSupportLayer.ServiceStack.Mapping;
using SevenDigital.ApiSupportLayer.ServiceStack.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Unit.Tests.Mapping
{
	[TestFixture]
	public class AddCardMapperTests
	{
		[Test]
		public void Should_map_fully()
		{
			var addCardMapper = new AddCardMapper();
			var addCardRequest = new AddCardRequest
			{
				ExpiryDate =  new DateTime(2020,12,31),
				HolderName = "Test Name",
				Id = 1234,
				IssueNumber = "1",
				Number = "1234123412341234",
				PostCode = "TQ9 7PY",
				StartDate = new DateTime(2000,01,01),
				TwoLetterISORegionName = "GB",
				Type = "VISA",
				VerificationCode = "267"
			};
			var addCardParameters = addCardMapper.Map(addCardRequest);

			Assert.That(addCardParameters.ExpiryDate, Is.EqualTo(addCardRequest.ExpiryDate));
			Assert.That(addCardParameters.HolderName, Is.EqualTo(addCardRequest.HolderName));
			Assert.That(addCardParameters.IssueNumber, Is.EqualTo(int.Parse(addCardRequest.IssueNumber)));
			Assert.That(addCardParameters.Number, Is.EqualTo(addCardRequest.Number));
			Assert.That(addCardParameters.PostCode, Is.EqualTo(addCardRequest.PostCode));
			Assert.That(addCardParameters.StartDate, Is.EqualTo(addCardRequest.StartDate));
			Assert.That(addCardParameters.TwoLetterISORegionName, Is.EqualTo(addCardRequest.TwoLetterISORegionName));
			Assert.That(addCardParameters.Type, Is.EqualTo(addCardRequest.Type));
			Assert.That(addCardParameters.VerificationCode, Is.EqualTo(addCardRequest.VerificationCode));
		}

		[Test]
		public void IssueNumber_and_start_date_are_null_if_missing_from_request()
		{
			var addCardMapper = new AddCardMapper();
			var addCardRequest = new AddCardRequest
			{
				ExpiryDate = new DateTime(2020, 12, 31),
				HolderName = "Test Name",
				Id = 1234,
				Number = "1234123412341234",
				PostCode = "TQ9 7PY",
				TwoLetterISORegionName = "GB",
				Type = "VISA",
				VerificationCode = "267"
			};
			var addCardParameters = addCardMapper.Map(addCardRequest);

			Assert.That(addCardParameters.ExpiryDate, Is.EqualTo(addCardRequest.ExpiryDate));
			Assert.That(addCardParameters.HolderName, Is.EqualTo(addCardRequest.HolderName));
			Assert.That(addCardParameters.IssueNumber, Is.Null);
			Assert.That(addCardParameters.Number, Is.EqualTo(addCardRequest.Number));
			Assert.That(addCardParameters.PostCode, Is.EqualTo(addCardRequest.PostCode));
			Assert.That(addCardParameters.StartDate, Is.Null);
			Assert.That(addCardParameters.TwoLetterISORegionName, Is.EqualTo(addCardRequest.TwoLetterISORegionName));
			Assert.That(addCardParameters.Type, Is.EqualTo(addCardRequest.Type));
			Assert.That(addCardParameters.VerificationCode, Is.EqualTo(addCardRequest.VerificationCode));
		}
	}
}