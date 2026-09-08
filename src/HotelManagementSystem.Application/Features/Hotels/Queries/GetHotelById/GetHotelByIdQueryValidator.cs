using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace HotelManagementSystem.Application.Features.Hotels.Queries.GetHotelById
{
    public sealed class GetHotelByIdQueryValidator : AbstractValidator<GetHotelByIdQuery>
    {
        public GetHotelByIdQueryValidator()
        {
            RuleFor(x => x.HotelId).NotEmpty();
        }
    }
}