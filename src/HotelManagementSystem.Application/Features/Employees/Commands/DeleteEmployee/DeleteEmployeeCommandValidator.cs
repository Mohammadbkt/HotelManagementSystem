using FluentValidation;

namespace HotelManagementSystem.Application.Features.Employees.Commands.DeleteEmployee
{
    public sealed class DeleteEmployeeCommandValidator
        : AbstractValidator<DeleteEmployeeCommand>
    {
        public DeleteEmployeeCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithErrorCode("Employee.IdRequired")
                .WithMessage("Employee id is required.");
        }
    }
}