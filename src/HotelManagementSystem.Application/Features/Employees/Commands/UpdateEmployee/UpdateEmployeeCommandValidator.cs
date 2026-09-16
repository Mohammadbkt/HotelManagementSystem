using FluentValidation;

namespace HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployee
{
    public sealed class UpdateEmployeeCommandValidator
        : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithErrorCode("Employee.IdRequired")
                .WithMessage("Employee id is required.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithErrorCode("Employee.FirstNameRequired")
                .WithMessage("First name is required.")
                .MaximumLength(100)
                .WithErrorCode("Employee.FirstNameTooLong")
                .WithMessage("First name must be 100 characters or fewer.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithErrorCode("Employee.LastNameRequired")
                .WithMessage("Last name is required.")
                .MaximumLength(100)
                .WithErrorCode("Employee.LastNameTooLong")
                .WithMessage("Last name must be 100 characters or fewer.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .WithErrorCode("Employee.DateOfBirthRequired")
                .WithMessage("Date of birth is required.")
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithErrorCode("Employee.DateOfBirthInFuture")
                .WithMessage("Date of birth cannot be in the future.")
                .GreaterThan(new DateOnly(1900, 1, 1))
                .WithErrorCode("Employee.DateOfBirthTooOld")
                .WithMessage("Date of birth is not valid.");
        }
    }
}