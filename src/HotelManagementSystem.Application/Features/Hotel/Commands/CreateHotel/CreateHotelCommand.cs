using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Hotel.Commands.CreateHotel
{
    public sealed record CreateHotelCommand(
    string Name,
    string Street,
    string City,
    string Country,
    string Phone
) : IRequest<Result<Guid>>;
}