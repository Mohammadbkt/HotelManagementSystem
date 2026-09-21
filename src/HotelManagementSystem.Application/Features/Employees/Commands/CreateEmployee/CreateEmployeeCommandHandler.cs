using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Employee;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Employees.Commands.CreateEmployee
{
    public sealed class CreateEmployeeCommandHandler(IAppDbContext context)
        : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken ct)
        {
            var hotelExists = await context.Hotels
                .AnyAsync(h => h.Id == request.HotelId, ct);

            if (!hotelExists)
                return Result<Guid>.Failure(ApplicationErrors.Hotel.NotFound);

            var emailResult = EmailAddress.Create(request.Email);
            if (emailResult.IsError)
                return Result<Guid>.Failure(emailResult.Errors);

            var email = emailResult.Value;

            PhoneNumber? phone = null;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var phoneResult = PhoneNumber.Create(request.PhoneNumber);
                if (phoneResult.IsError)
                    return Result<Guid>.Failure(phoneResult.Errors);

                phone = phoneResult.Value;
            }

            var emailString = email.Value;
            var phoneString = phone?.Value;

            var duplicate = await context.Employees.AnyAsync(e =>
                e.EmailAddress.Value == emailString ||
                (phoneString != null
                    && e.PhoneNumber != null
                    && e.PhoneNumber.Value.Value == phoneString),
                ct);

            if (duplicate)
                return Result<Guid>.Failure(
                    Error.Conflict(
                        "Employee.Duplicate",
                        "An employee with this email or phone already exists."));

            var employeeResult = Employee.Create(
                request.HotelId,
                request.FirstName,
                request.LastName,
                phone,
                email,
                request.Role);

            if (employeeResult.IsError)
                return Result<Guid>.Failure(employeeResult.Errors);

            await context.Employees.AddAsync(employeeResult.Value, ct);
            await context.SaveChangesAsync(ct);

            return Result<Guid>.Success(employeeResult.Value.Id);
        }
    }
}