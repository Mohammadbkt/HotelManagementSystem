using FluentValidation;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;

namespace HotelManagementSystem.Application.Features.Employees.Commands.CreateEmployee
{
    public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(x => x.HotelId)
                .NotEmpty()
                .WithErrorCode("Employee.HotelIdRequired")
                .WithMessage("Hotel id is required.");

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

            // Email is required — both presence and format.
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithErrorCode("Employee.EmailRequired")
                .WithMessage("Email is required.");

            RuleFor(x => x.Email)
                .Must(BeAValidEmail)
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithErrorCode("Employee.EmailInvalid")
                .WithMessage("Email is invalid.");

            // Phone is optional — only validate format when present.
            RuleFor(x => x.PhoneNumber)
                .Must(BeAValidPhoneNumber)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithErrorCode("Employee.PhoneNumberInvalid")
                .WithMessage("Phone number is invalid.");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithErrorCode("Employee.RoleInvalid")
                .WithMessage("Role is invalid.");
        }

        private static bool BeAValidEmail(string? email)
            => !string.IsNullOrWhiteSpace(email)
               && !EmailAddress.Create(email).IsError;

        private static bool BeAValidPhoneNumber(string? phone)
            => !string.IsNullOrWhiteSpace(phone)
               && !PhoneNumber.Create(phone).IsError;
    }
}