using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.DeactivateRoomCategory
{
    public sealed record DeactivateRoomCategoryCommand(
        Guid RoomCategoryId
    ) : IRequest<Result>;
}