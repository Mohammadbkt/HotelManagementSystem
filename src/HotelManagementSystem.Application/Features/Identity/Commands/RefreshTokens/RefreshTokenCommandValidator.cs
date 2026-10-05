using FluentValidation;

namespace HotelManagementSystem.Application.Features.Identity.Commands.RefreshTokens
{
    public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty();
        }
    }
}