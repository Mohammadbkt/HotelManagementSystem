using FluentValidation;

namespace HotelManagementSystem.Application.Features.RoomCategories.Queries.GetRoomCategoryById
{
    public sealed class GetRoomCategoryByIdQueryValidator : AbstractValidator<GetRoomCategoryByIdQuery>
    {
        public GetRoomCategoryByIdQueryValidator()
        {
            RuleFor(x => x.RoomCategoryId)
                .NotEmpty();
        }
    }
}