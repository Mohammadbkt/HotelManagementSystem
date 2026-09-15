using HotelManagementSystem.Application.Features.Stays.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Stays.Queries.GetStayById
{
    public sealed record GetStayByIdQuery(
        Guid StayId
    ) : IRequest<Result<StayDto>>;
}