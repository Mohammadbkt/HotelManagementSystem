using HotelManagementSystem.Application.Features.Hotels.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Hotels.Queries.GetHotelById
{
    public sealed record GetHotelByIdQuery(Guid HotelId) : IRequest<Result<HotelDto>>;
    
}