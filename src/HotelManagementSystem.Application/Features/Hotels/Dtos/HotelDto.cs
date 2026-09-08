namespace HotelManagementSystem.Application.Features.Hotels.Dtos
{
    public sealed record HotelDto(
        Guid Id,
        string Name,
        string AddressLine1,
        string? AddressLine2,
        string City,
        string? StateOrProvince,
        string? PostalCode,
        string Country,
        string PhoneNumber
    );

}