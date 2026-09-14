using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.ConfirmReservation
{
    public sealed record ConfirmReservationCommand(
        Guid ReservationId
    ) : IRequest<Result>;
}