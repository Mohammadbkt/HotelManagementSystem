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
                .MaximumLength(500)
                .When(x => x.Description is not null);

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