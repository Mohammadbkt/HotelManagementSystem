using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuest
{
    public sealed record UpdateGuestInfoCommand(
        Guid GuestId,
        string FirstName,
        string LastName,
        DateOnly DateOfBirth
    ) : IRequest<Result>;
}