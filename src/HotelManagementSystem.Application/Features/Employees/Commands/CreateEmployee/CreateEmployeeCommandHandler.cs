using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Employee;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Employees.Commands.CreateEmployee
{
    public sealed class CreateEmployeeCommandHandler(IAppDbContext context, ILogger<CreateEmployeeCommandHandler> logger)
        : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
    {

        private readonly IAppDbContext _context = context;
        private readonly ILogger<CreateEmployeeCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var hotelExists = await _context.Hotels
                .AnyAsync(h => h.Id == request.HotelId, cancellationToken);

            if (!hotelExists)
            {
                _logger.LogWarning(
                   "Hotel {HotelId} not found; cannot create employee",
                   request.HotelId);

                return Result<Guid>.Failure(ApplicationErrors.Hotel.NotFound);
            }
            var emailResult = EmailAddress.Create(request.Email!);
            if (emailResult.IsError)
            {
                _logger.LogWarning(
                   "Invalid email {Email} when creating employee for hotel {HotelId}: {Errors}",
                   request.Email,
                   request.HotelId,
                   emailResult.Errors);

                return Result<Guid>.Failure(emailResult.Errors);
            }
            var email = emailResult.Value;

            PhoneNumber? phone = null;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var phoneResult = PhoneNumber.Create(request.PhoneNumber);
                if (phoneResult.IsError)
                {
                    _logger.LogWarning(
                       "Invalid phone {PhoneNumber} when creating employee for hotel {HotelId}: {Errors}",
                       request.PhoneNumber,
                       request.HotelId,
                       phoneResult.Errors);

                    return Result<Guid>.Failure(phoneResult.Errors);
                }
                phone = phoneResult.Value;
            }

            var emailString = email.Value;
            var phoneString = phone?.Value;

            var duplicate = await _context.Employees.AnyAsync(e =>
                e.EmailAddress.Value == emailString ||
                (phoneString != null
                    && e.PhoneNumber != null
                    && e.PhoneNumber.Value.Value == phoneString),
                cancellationToken);

            if (duplicate)
            {
                _logger.LogWarning(
                   "Duplicate employee detected for hotel {HotelId} (email {Email}, phone {PhoneNumber})",
                   request.HotelId,
                   emailString,
                   phoneString);

                return Result<Guid>.Failure(ApplicationErrors.Employee.AlreadyExists);
            }
            var employeeResult = Employee.Create(
                request.HotelId,
                request.FirstName,
                request.LastName,
                phone,
                email,
                request.Role);

            if (employeeResult.IsError)
            {
                _logger.LogWarning(
                    "Employee.Create failed for hotel {HotelId} with email {Email}: {Errors}",
                    request.HotelId,
                    emailString,
                    employeeResult.Errors);

                return Result<Guid>.Failure(employeeResult.Errors);
            }
            await _context.Employees.AddAsync(employeeResult.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
               "Employee {EmployeeId} created for hotel {HotelId} with email {Email}",
               employeeResult.Value.Id,
               request.HotelId,
               emailString);

            return Result<Guid>.Success(employeeResult.Value.Id);
        }
    }
}