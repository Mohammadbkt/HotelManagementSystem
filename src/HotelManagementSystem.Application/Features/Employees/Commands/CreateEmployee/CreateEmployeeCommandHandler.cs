using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Common.Models;
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
         IUserService userService)
        : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
    {

        private readonly IAppDbContext _context = context;
        private readonly ILogger<CreateEmployeeCommandHandler> _logger = logger;
        private readonly IUserService _userService = userService;

        public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var hotelExists = await _context.Hotels
                .AnyAsync(h => h.Id == request.HotelId, cancellationToken);

            if (!hotelExists)
            {
                return Result<Guid>.Failure(ApplicationErrors.Hotel.NotFound);
            }

            var emailResult = EmailAddress.Create(request.Email);
            if (emailResult.IsError) return Result<Guid>.Failure(emailResult.Errors);
            var email = emailResult.Value;

            PhoneNumber? phone = null;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var phoneResult = PhoneNumber.Create(request.PhoneNumber);
                if (phoneResult.IsError) return Result<Guid>.Failure(phoneResult.Errors);
                phone = phoneResult.Value;
            }
            var phoneValue = phone?.Value;

            var duplicate = await _context.Employees.AnyAsync(e => e.EmailAddress.Value == email.Value ||
                    (phoneValue != null && e.PhoneNumber != null && e.PhoneNumber.Value == phoneValue), cancellationToken);

            if (duplicate)
            {
                return Result<Guid>.Failure(ApplicationErrors.Employee.AlreadyExists);
            }

            Guid employeeId;

            var createUserRequest = new CreateUserRequest(
            UserName: request.Email,
            Email: request.Email,
            PhoneNumber: phoneValue,
            FirstName: request.FirstName,
            LastName: request.LastName,
            Password: request.Password);


            await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var userResult = await _userService.CreateUserAsync(createUserRequest, cancellationToken);
                if (userResult.IsError)
                    return Result<Guid>.Failure(userResult.Errors);

                var user = userResult.Value;

                var roleResult = await _userService.AssignRoleAsync(user.Id, request.Role.ToString(), cancellationToken);
                if (roleResult.IsError)
                    return Result<Guid>.Failure(roleResult.Errors);

                var employeeResult = Employee.Create(request.HotelId, user.Id, request.FirstName,
                    request.LastName, phone, email, request.DateOfBirth, request.Role);
                if (employeeResult.IsError)
                    return Result<Guid>.Failure(employeeResult.Errors);

                _context.Employees.Add(employeeResult.Value);

                await _context.SaveChangesAsync(cancellationToken);

                await tx.CommitAsync(cancellationToken);

                employeeId = employeeResult.Value.Id;

            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Employee creation failed for hotel {HotelId}", request.HotelId);
                return Result<Guid>.Failure(Error.Failure("Employee.CreateFailed", "Employee could not be created. Please try again."));
            }   

            await _userService.SendInviteAsync(email.Value, cancellationToken);

            return Result<Guid>.Success(employeeId);
        }
    }
}