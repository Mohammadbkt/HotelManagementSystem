using FluentValidation;
using HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployeeContactInfo.HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployeeContactInfo;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;

namespace HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployeeContactInfo
{
    public sealed class UpdateEmployeeContactInfoCommandValidator
        : AbstractValidator<UpdateEmployeeContactInfoCommand>
    {
        public UpdateEmployeeContactInfoCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithErrorCode("Employee.IdRequired")
                .WithMessage("Employee id is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithErrorCode("Employee.EmailRequired")
                .WithMessage("Email is required.");

            RuleFor(x => x.Email)
                .Must(BeAValidEmail)
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithErrorCode("Employee.EmailInvalid")
                .WithMessage("Email is invalid.");

            RuleFor(x => x.PhoneNumber)
                .Must(BeAValidPhoneNumber)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithErrorCode("Employee.PhoneNumberInvalid")
                .WithMessage("Phone number is invalid.");
        }

        private static bool BeAValidEmail(string? email)
            => !string.IsNullOrWhiteSpace(email)
               && !EmailAddress.Create(email).IsError;

        private static bool BeAValidPhoneNumber(string? phone)
            => !string.IsNullOrWhiteSpace(phone)
               && !PhoneNumber.Create(phone).IsError;
    }
}