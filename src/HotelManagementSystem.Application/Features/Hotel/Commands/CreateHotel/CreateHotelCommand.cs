using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Hotels.Commands.CreateHotel
{
    public sealed record CreateHotelCommand(
    string Name,
    string Street,
    string City,
    string Country,
    string Phone
) : IRequest<Result<Guid>>;
}