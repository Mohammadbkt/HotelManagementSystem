using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuestContactInfo
{
    public sealed record UpdateGuestContactInfoCommand(
        Guid GuestId,
        string? Email,
        string? Phone
    ) : IRequest<Result>;
}