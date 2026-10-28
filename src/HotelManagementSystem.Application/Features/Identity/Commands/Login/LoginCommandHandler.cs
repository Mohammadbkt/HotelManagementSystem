using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Identity.Commands.Login
{
    public sealed class LoginCommandHandler(
            IAuthService authService,
            ILogger<LoginCommandHandler> logger
            ) : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IAuthService _authService = authService;
        private readonly ILogger<LoginCommandHandler> _logger = logger;

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request.Identifier, request.Password, request.DeviceName, request.IpAddress, cancellationToken);
            if (result.IsError)
                _logger.LogWarning("Failed login attempt: {Error}", result.TopError.Code);
            
            return result;


        }
    }
}