using HotelManagementSystem.Application.Features.Guests.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Guests.Queries.GetGuestById
{
    public sealed record GetGuestByIdQuery(
        Guid GuestId
    ) : IRequest<Result<GuestDto>>;
}