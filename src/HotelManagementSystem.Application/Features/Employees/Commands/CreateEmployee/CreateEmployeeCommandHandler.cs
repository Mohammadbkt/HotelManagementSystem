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
    public sealed class CreateEmployeeCommandHandler(
         IAppDbContext context,
         ILogger<CreateEmployeeCommandHandler> logger,
         IIdentityService identityService)
        : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
    {

        private readonly IAppDbContext _context = context;
        private readonly ILogger<CreateEmployeeCommandHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;

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

            var employeeExists = await _context.Employees.AnyAsync(e => e.UserId == request.UserId, cancellationToken);

            if (employeeExists)
            {
                _logger.LogInformation("There is already an employee with {UserId}", request.UserId);
                return Result<Guid>.Failure(ApplicationErrors.Employee.AlreadyExists);
            }

            var userExists = await _identityService.GetUserByIdAsync(request.UserId, cancellationToken);

            if (userExists.IsError)
            {
                _logger.LogWarning(
                   "User {UserId} not found; cannot create employee for hotel {HotelId}",
                   request.UserId,
                   request.HotelId);

                return Result<Guid>.Failure(Error.Validation("User.NotFound", "The specified user does not exist."));
            }

            var user = userExists.Value;

            var roleExists = await _identityService.RoleExistsAsync(request.Role.ToString());
            if (!roleExists)
            {
                _logger.LogWarning(
                   "Role {Role} does not exist; cannot create employee for hotel {HotelId}",
                   request.Role,
                   request.HotelId);

                return Result<Guid>.Failure(Error.Validation("Role.NotFound", "The specified role does not exist."));
            }

            var emailResult = EmailAddress.Create(user.Email!);
            if (emailResult.IsError)
            {
                _logger.LogWarning(
                   "Invalid email {Email} when creating employee for hotel {HotelId}: {Errors}",
                   user.Email,
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


            var duplicate = await _context.Employees.AnyAsync(e =>
                e.EmailAddress.Value == email.Value ||
                (phone != null
                    && e.PhoneNumber != null
                    && e.PhoneNumber.Value == phone.Value),
                cancellationToken);

            if (duplicate)
            {
                _logger.LogWarning(
                   "Duplicate employee detected for hotel {HotelId} (email {Email}, phone {PhoneNumber})",
                   request.HotelId,
                   email,
                   phone);

                return Result<Guid>.Failure(ApplicationErrors.Employee.AlreadyExists);
            }

            var employeeResult = Employee.Create(
                request.HotelId,
                request.UserId,
                request.FirstName,
                request.LastName,
                phone,
                email,
                request.DateOfBirth,
                request.Role);

            if (employeeResult.IsError)
            {
                _logger.LogWarning(
                    "Employee.Create failed for hotel {HotelId} with email {Email}: {Errors}",
                    request.HotelId,
                    email,
                    employeeResult.Errors);

                return Result<Guid>.Failure(employeeResult.Errors);
            }

            var roleResult = await _identityService.SetRoleAsync(request.UserId, request.Role.ToString(), cancellationToken);
           
            if (roleResult.IsError) 
                return Result<Guid>.Failure(roleResult.Errors);


            await _context.Employees.AddAsync(employeeResult.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
               "Employee {EmployeeId} created for hotel {HotelId} with email {Email}",
               employeeResult.Value.Id,
               request.HotelId,
               email);

            return Result<Guid>.Success(employeeResult.Value.Id);
        }
    }
}