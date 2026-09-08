using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Hotels.Commands.CreateHotel
{
    public sealed record CreateHotelCommand(
    string Name,
    string AddressLine1,
    string City,
    string Country,
    string? AddressLine2,
    string? StateOrProvince,
    string? PostalCode,
    string PhoneNumber
) : IRequest<Result<Guid>>;
}