using FluentValidation;
using HotelManagementSystem.Application.Features.Stays.Commands.CheckOutGuest;

namespace HotelManagementSystem.Application.Features.Stays.Commands.CheckOutStay
{
    public sealed class CheckOutStayCommandValidator : AbstractValidator<CheckOutGuestCommand>
    {
        public CheckOutStayCommandValidator()
        {
            RuleFor(x => x.StayId)
                .NotEmpty();
        }
    }
}