using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.CreateRoom
{
    public sealed record CreateRoomCommand(
        
    ) : IRequest<Result<Guid>>;
    
}