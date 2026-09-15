using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Stays.Commands.CheckOutGuest
{
    public sealed record CheckOutGuestCommand(
        Guid StayId
    ) : IRequest<Result>;
}