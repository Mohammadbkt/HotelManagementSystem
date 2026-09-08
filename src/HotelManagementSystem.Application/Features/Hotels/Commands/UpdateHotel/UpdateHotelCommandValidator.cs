using FluentValidation;

namespace HotelManagementSystem.Application.Features.Hotels.Commands.UpdateHotel
{
    public sealed class UpdateHotelCommandValidator : AbstractValidator<UpdateHotelCommand>
    {
        public UpdateHotelCommandValidator()
        {
            RuleFor(x => x.HotelId).NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.AddressLine1)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.AddressLine2)
                .MaximumLength(200)
                .When(x => x.AddressLine2 is not null);

            RuleFor(x => x.City)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.StateOrProvince)
                .MaximumLength(100)
                .When(x => x.StateOrProvince is not null);

            RuleFor(x => x.PostalCode)
                .MaximumLength(20)
                .When(x => x.PostalCode is not null);

            RuleFor(x => x.Country)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty();
        }
    }
}