using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployeeContactInfo
{
    public sealed class UpdateEmployeeContactInfoCommandHandler(IAppDbContext context, ILogger<UpdateEmployeeContactInfoCommandHandler> logger)
        : IRequestHandler<UpdateEmployeeContactInfoCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<UpdateEmployeeContactInfoCommandHandler> _logger = logger;


        public async Task<Result> Handle(
            UpdateEmployeeContactInfoCommand request,
            CancellationToken cancellationToken)
        {
            var employee = await _context.Employees.FindAsync([request.EmployeeId], cancellationToken);

            if (employee is null)
            {
                _logger.LogWarning(
                    "Employee {EmployeeId} not found; cannot update employee contact info",
                    request.EmployeeId);

                return Result.Failure(ApplicationErrors.Employee.NotFound);
            }
            var emailResult = EmailAddress.Create(request.Email);
            if (emailResult.IsError)
            {

                _logger.LogWarning(
                   "Invalid email {Email} when creating employee, {Errors}",
                   request.Email,
                   emailResult.Errors);

                return Result.Failure(emailResult.Errors);
            }
            var email = emailResult.Value;

            PhoneNumber? phone = null;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var phoneResult = PhoneNumber.Create(request.PhoneNumber);
                if (phoneResult.IsError)
                {
                    _logger.LogWarning(
                        "Invalid phone {PhoneNumber} when creating employee, {Errors}",
                        request.PhoneNumber,
                        phoneResult.Errors);

                    return Result.Failure(phoneResult.Errors);
                }
                phone = phoneResult.Value;
            }

            var emailString = email.Value;
            var phoneString = phone?.Value;

            var duplicate = await _context.Employees.AnyAsync(
                e => e.Id != request.EmployeeId && (e.EmailAddress.Value == emailString ||
                 (phoneString != null && e.PhoneNumber != null && e.PhoneNumber.Value.Value == phoneString)),
                cancellationToken);

            if (duplicate)
            {
                _logger.LogWarning(
                    "Duplicate contact info for employee {EmployeeId} (email {Email}, phone {PhoneNumber})",
                    request.EmployeeId,
                    emailString,
                    phoneString);

                return Result.Failure(ApplicationErrors.Employee.AlreadyExists);
            }
            var contactResult = employee.UpdateContactInfo(phone, email);
            if (contactResult.IsError)
            {
                _logger.LogWarning(
                    "Failed to update contact info for employee {EmployeeId}: {Errors}",
                    request.EmployeeId,
                    contactResult.Errors);

                return contactResult;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Employee {EmployeeId} contact info updated (email {Email}, phone {PhoneNumber})",
                request.EmployeeId,
                emailString,
                phoneString);

            return Result.Success();
        }
    }
}