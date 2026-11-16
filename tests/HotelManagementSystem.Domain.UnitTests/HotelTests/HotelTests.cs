using HotelManagementSystem.Domain.ValueObjects.Address;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using HotelManagementSystem.Domain.Entities.Hotels;
using HotelManagementSystem.Domain.Common.Result;

using Xunit;


namespace HotelManagementSystem.Domain.UnitTests.HotelTests
{
    public class HotelTests
    {
        private static Address ValidAddress() => Address.Create(
            addressLine1: "1 Main St",
            city: "Springfield",
            country: "USa",
            null,
            null,
            postalCode: "62701"
            ).Value;

        private static PhoneNumber ValidPhone() => PhoneNumber.Create("+962799560107").Value;

        [Fact]
        public void Create_WithValidInput_ReturnsSuccessAndPopulatedHotel()
        {

            var address = ValidAddress();
            var phone = ValidPhone();

            var result = Hotel.Create("Grand Plaza", address, phone);

            Assert.True(result.IsSuccess);
            Assert.False(result.IsError);
            Assert.NotNull(result.Value);
            Assert.Equal("Grand Plaza", result.Value.Name);
            Assert.Equal(address, result.Value.Address);
            Assert.Equal(phone, result.Value.PhoneNumber);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidName_ReturnsFailureWithNameRequiredCode(string? invalidName)
        {
            var result = Hotel.Create(invalidName!, ValidAddress(), ValidPhone());

            Assert.True(result.IsError);
            Assert.Equal("Hotel.NameRequired", result.TopError.Code);
        }




        [Fact]
        public void UpdateInfo_WithValidInput_UpdatesHotelProperties()
        {

            var address = ValidAddress();
            var phone = ValidPhone();


            var hotelResult = Hotel.Create("Grand Plaza", address, phone);

            var newAddress = Address.Create(
                addressLine1: "25 King Street",
                city: "Amman",
                country: "Jordan",
                null,
                null,
                postalCode: "11118").Value;

            var newPhone = PhoneNumber.Create("+962799123456").Value;


            hotelResult.Value.UpdateInfo("Royal Hotel",
                    newAddress,
                    newPhone);


            Assert.True(hotelResult.IsSuccess);
            Assert.False(hotelResult.IsError);

            Assert.Equal("Royal Hotel", hotelResult.Value.Name);
            Assert.Equal(newAddress, hotelResult.Value.Address);
            Assert.Equal(newPhone, hotelResult.Value.PhoneNumber);

        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateInfo_WithInvalidName_ReturnsFailureAndPreservesExistingProperties(
    string? invalidName)
        {
            // Arrange
            var originalAddress = ValidAddress();
            var originalPhone = ValidPhone();

            var hotel = Hotel.Create(
                "Grand Plaza",
                originalAddress,
                originalPhone).Value;

            var newAddress = Address.Create(
                "25 King Street",
                "Amman",
                "Jordan",
                null,
                null,
                "11118").Value;

            var newPhone = PhoneNumber.Create("+962799123456").Value;

            // Act
            var result = hotel.UpdateInfo(
                invalidName!,
                newAddress,
                newPhone);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Hotel.NameRequired", result.TopError.Value.Code);

            // Verify that nothing changed
            Assert.Equal("Grand Plaza", hotel.Name);
            Assert.Equal(originalAddress, hotel.Address);
            Assert.Equal(originalPhone, hotel.PhoneNumber);
        }

        [Fact]
        public void UpdateInfo_WithNameContainingWhitespace_TrimsName()
        {
            // Arrange
            var hotel = Hotel.Create(
                "Grand Plaza",
                ValidAddress(),
                ValidPhone()).Value;

            // Act
            var result = hotel.UpdateInfo(
                "  Royal Hotel  ",
                ValidAddress(),
                ValidPhone());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Royal Hotel", hotel.Name);
        }
    }
}