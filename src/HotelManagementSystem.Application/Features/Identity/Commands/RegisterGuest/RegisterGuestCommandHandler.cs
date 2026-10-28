using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Identity.Commands.RegisterGuest;

public sealed class RegisterGuestCommandHandler(
    IAppDbContext context,
    IUserService userService,
    ILogger<RegisterGuestCommandHandler> logger)
    : IRequestHandler<RegisterGuestCommand, Result<RegisterGuestResponseDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly IUserService _userService = userService;
    private readonly ILogger<RegisterGuestCommandHandler> _logger = logger;

    public async Task<Result<RegisterGuestResponseDto>> Handle(
        RegisterGuestCommand request,
        CancellationToken cancellationToken)
    {
        var emailResult = EmailAddress.Create(request.Email);

        if (emailResult.IsError)
            return Result<RegisterGuestResponseDto>.Failure(emailResult.Errors);

        PhoneNumber? phone = null;

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneResult = PhoneNumber.Create(request.PhoneNumber);

            if (phoneResult.IsError)
                return Result<RegisterGuestResponseDto>.Failure(phoneResult.Errors);

            phone = phoneResult.Value;
        }

        var email = emailResult.Value.Value;
        var phoneValue = phone?.Value;

        var matches = await _context.Guests
            .Where(g => g.Email != null && g.Email.Value.Value == email)
            .Take(2)
            .ToListAsync(cancellationToken);

        if (matches.Count > 1)
            return Result<RegisterGuestResponseDto>.Failure(GuestErrors.AmbiguousMatch);

        var guest = matches.SingleOrDefault();

        if (guest is not null && guest.UserId is not null)
            return Result<RegisterGuestResponseDto>.Failure(ApplicationErrors.GuestErrors.AlreadyExists);

        var isNewGuest = guest is null;

        if (guest is null)
        {
            var guestResult = Guest.Create(
                request.FirstName,
                request.LastName,
                emailResult.Value,
                phone,
                request.DateOfBirth);

            if (guestResult.IsError)
                return Result<RegisterGuestResponseDto>.Failure(guestResult.Errors);

            guest = guestResult.Value;
        }

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
            {
                return Result<RegisterGuestResponseDto>.Failure(userResult.Errors);
            }

            var user = userResult.Value;

            var roleResult = await _userService.AssignRoleAsync(user.Id, Roles.Guest, cancellationToken);

            if (roleResult.IsError)
            {
                return Result<RegisterGuestResponseDto>.Failure(roleResult.Errors);
            }


            var linkResult = guest.LinkToUserAccount(user.Id);

            if (linkResult.IsError)
            {
                return Result<RegisterGuestResponseDto>.Failure(linkResult.Errors);
            }

            if (isNewGuest) _context.Guests.Add(guest);

            await _context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch (System.Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Guest registration failed for {Email}", email);
            return Result<RegisterGuestResponseDto>.Failure(
                Error.Failure("Register.SaveFailed", "Registration could not be completed. Please try again."));

        }

        await _userService.SendEmailConfirmationAsync(email, cancellationToken);

        return Result<RegisterGuestResponseDto>.Success(
            new RegisterGuestResponseDto(email, true)
            );
    }
}
