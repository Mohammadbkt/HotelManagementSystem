using FluentValidation;
using HotelManagementSystem.Domain.Entities.Rooms;

namespace HotelManagementSystem.Application.Features.Rooms.Queries.GetRooms
{
    public sealed class GetRoomsQueryValidator : AbstractValidator<GetRoomsQuery>
    {
        public GetRoomsQueryValidator()
        {
            RuleFor(x => x.HotelId)
                .NotEmpty()
                .WithMessage("HotelId is required.");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm))
                .WithMessage("Search term cannot exceed 200 characters.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue)
                .WithMessage("Invalid room status.");


            RuleFor(x => x.SortDirection)
                .IsInEnum()
                .WithMessage("Invalid sort direction.");

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("PageNumber must be at least 1.");

        }

    }
}