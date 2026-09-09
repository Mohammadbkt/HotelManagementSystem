using FluentValidation;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.UpdateBasePriceForRoomCategory
{
    public class UpdateRoomCategoryBasePriceCommandValidator : AbstractValidator<UpdateRoomCategoryBasePriceCommand>
    {
        public UpdateRoomCategoryBasePriceCommandValidator()
        {
            RuleFor(x => x.RoomCategoryId)
                .NotEmpty();

            RuleFor(x => x.Price)
                .GreaterThan(0);

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3);
        }
    }
}