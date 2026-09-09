using FluentValidation;

namespace HotelManagementSystem.Application.Features.RoomCategories.Queries.GetRoomCategoryById
{
    public sealed class GetRoomCategoryByIdCommandValidator : AbstractValidator<GetRoomCategoryByIdCommand>
    {
        public GetRoomCategoryByIdCommandValidator()
        {
            RuleFor(x => x.RoomCategoryId)
                .NotEmpty();
        }
    }
}