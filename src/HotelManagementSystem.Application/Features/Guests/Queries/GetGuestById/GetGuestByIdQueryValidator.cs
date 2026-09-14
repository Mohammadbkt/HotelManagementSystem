using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace HotelManagementSystem.Application.Features.Guests.Queries.GetGuestById
{
    public sealed class GetGuestByIdQueryValidator : AbstractValidator<GetGuestByIdQuery>
    {
        public GetGuestByIdQueryValidator()
        {
            RuleFor(x => x.GuestId)
                .NotEmpty();
        }
    }
}