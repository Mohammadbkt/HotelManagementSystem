using FluentValidation;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.CreateRoomCategory
{
    public sealed class CreateRoomCategoryCommandValidator : AbstractValidator<CreateRoomCategoryCommand>
    {
        public CreateRoomCategoryCommandValidator()
        {
            RuleFor(x => x.HotelId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.MaxCapacity)
                .GreaterThan(0);

            RuleFor(x => x.BasePrice)
                .GreaterThan(0);

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3);
        }
    }
}