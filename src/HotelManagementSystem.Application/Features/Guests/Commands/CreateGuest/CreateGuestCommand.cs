using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Guests.Commands.CreateGuest
{
    public sealed record CreateGuestCommand(
        string FirstName,
        string LastName,
        string? Email,
        string? Phone,
        DateOnly DateOfBith
    ) : IRequest<Result<Guid>>;
}