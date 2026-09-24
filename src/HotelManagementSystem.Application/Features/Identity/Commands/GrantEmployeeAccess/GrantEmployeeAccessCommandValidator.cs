using FluentValidation;

namespace HotelManagementSystem.Application.Features.Identity.Commands.GrantEmployeeAccess
{
    public sealed class GrantEmployeeAccessCommandValidator : AbstractValidator<GrantEmployeeAccessCommand>
    {
        public GrantEmployeeAccessCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty();

            RuleFor(x => x.TemporaryPassword)
                .NotEmpty()
                .MinimumLength(8);
        }
    }
}