using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.DeactivateRoomCategory
{
    public sealed class DeactivateRoomCategoryCommandHandler(IAppDbContext context, ICurrentUserService currentUser, ILogger<DeactivateRoomCategoryCommandHandler> logger) : IRequestHandler<DeactivateRoomCategoryCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ICurrentUserService _currentUser = currentUser;
        private readonly ILogger<DeactivateRoomCategoryCommandHandler> _logger = logger;

        public async Task<Result> Handle(DeactivateRoomCategoryCommand request, CancellationToken cancellationToken)
        {
            var roomCategory = await _context.RoomCategories.FindAsync([request.RoomCategoryId], cancellationToken);
            if (roomCategory is null)
            {
                _logger.LogWarning("Room category {RoomCategoryId} not found", request.RoomCategoryId);
                return Result.Failure(ApplicationErrors.RoomCategory.NotFound);
            }
           
            if (_currentUser.UserId is not { } userId)
            {
                _logger.LogWarning("User not authenticated while deactivating room category {RoomCategoryId}", request.RoomCategoryId);
                return Result.Failure(ApplicationErrors.Auth.NotAuthenticated);
            }

            roomCategory.Delete(userId);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Room category {RoomCategoryId} deactivated by user {UserId}", request.RoomCategoryId, userId);
            return Result.Success();



        }
    }
}