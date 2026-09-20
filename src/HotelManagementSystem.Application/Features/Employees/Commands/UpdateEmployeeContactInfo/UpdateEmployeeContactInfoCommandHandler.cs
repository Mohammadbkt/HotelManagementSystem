using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployeeContactInfo.HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployeeContactInfo;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployeeContactInfo
{
    public sealed class UpdateEmployeeContactInfoCommandHandler(IAppDbContext context)
        : IRequestHandler<UpdateEmployeeContactInfoCommand, Result>
    {
        public async Task<Result> Handle(
            UpdateEmployeeContactInfoCommand request,
            CancellationToken cancellationToken)
        {
            var employee = await context.Employees.FindAsync([request.EmployeeId], cancellationToken);

            if (employee is null)
                return Result.Failure(Error.NotFound("Employee.NotFound", "Employee was not found."));

            var emailResult = EmailAddress.Create(request.Email);
            if (emailResult.IsError)
                return Result.Failure(emailResult.Errors);
            var email = emailResult.Value;

            PhoneNumber? phone = null;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var phoneResult = PhoneNumber.Create(request.PhoneNumber);
                if (phoneResult.IsError)
                    return Result.Failure(phoneResult.Errors);
                phone = phoneResult.Value;
            }

            var emailString = email.Value;
            var phoneString = phone?.Value;

            var duplicate = await context.Employees.AnyAsync(
                e =>e.Id != request.EmployeeId && (e.EmailAddress.Value == emailString ||
                 (phoneString != null && e.PhoneNumber != null && e.PhoneNumber.Value.Value == phoneString)),
                cancellationToken);

            if (duplicate)
                return Result.Failure(Error.Conflict("Employee.Duplicate","An employee with this email or phone already exists."));

            var contactResult = employee.UpdateContactInfo(phone, email);
            if (contactResult.IsError)
                return contactResult;

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}