using FluentValidation;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.DeactivateRoomCategory
{
    public sealed class DeactivateRoomCategoryCommandValidator : AbstractValidator<DeactivateRoomCategoryCommand>
    {
        public DeactivateRoomCategoryCommandValidator()
        {
            RuleFor(x => x.RoomCategoryId)
                .NotEmpty();
        }
    }
}