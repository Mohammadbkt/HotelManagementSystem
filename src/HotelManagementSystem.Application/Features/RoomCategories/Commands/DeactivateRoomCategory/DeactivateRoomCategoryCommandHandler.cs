using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.DeactivateRoomCategory
{
    public sealed class DeactivateRoomCategoryCommandHandler(IAppDbContext context, ICurrentUserService currentUser) : IRequestHandler<DeactivateRoomCategoryCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ICurrentUserService _currentUser = currentUser;

        public async Task<Result> Handle(DeactivateRoomCategoryCommand request, CancellationToken cancellationToken)
        {
            var roomCategory = await _context.RoomCategories.FindAsync([request.RoomCategoryId], cancellationToken);
            if (roomCategory is null)
                return Result.Failure(ApplicationErrors.RoomCategory.NotFound);
           
            if (_currentUser.UserId is not { } userId)
                return Result.Failure(ApplicationErrors.Auth.NotAuthenticated);

            roomCategory.Delete(userId);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();



        }
    }
}