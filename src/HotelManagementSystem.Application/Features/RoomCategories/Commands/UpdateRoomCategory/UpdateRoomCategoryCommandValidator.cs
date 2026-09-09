using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.UpdateRoomCategory
{
    public class UpdateRoomCategoryCommandValidator : AbstractValidator<UpdateRoomCategoryCommand>
    {
        public UpdateRoomCategoryCommandValidator()
        {
            RuleFor(x => x.RoomCategoryId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => x.Description is not null);

            RuleFor(x => x.MaxCapacity)
                .GreaterThan(0);
        }
    }
}