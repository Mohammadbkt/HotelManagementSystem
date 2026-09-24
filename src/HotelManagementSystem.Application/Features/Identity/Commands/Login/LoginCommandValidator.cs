using FluentValidation;

namespace HotelManagementSystem.Application.Features.Identity.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Identifier)
            .NotEmpty()
            .MaximumLength(256);

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}