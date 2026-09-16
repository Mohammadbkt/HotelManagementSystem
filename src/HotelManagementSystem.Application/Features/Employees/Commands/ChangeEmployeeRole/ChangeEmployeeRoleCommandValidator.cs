using FluentValidation;

namespace HotelManagementSystem.Application.Features.Employees.Commands.ChangeEmployeeRole
{
    public sealed class ChangeEmployeeRoleCommandValidator
        : AbstractValidator<ChangeEmployeeRoleCommand>
    {
        public ChangeEmployeeRoleCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithErrorCode("Employee.IdRequired")
                .WithMessage("Employee id is required.");

            RuleFor(x => x.NewRole)
                .IsInEnum()
                .WithErrorCode("Employee.RoleInvalid")
                .WithMessage("Role is invalid.");
        }
    }
}