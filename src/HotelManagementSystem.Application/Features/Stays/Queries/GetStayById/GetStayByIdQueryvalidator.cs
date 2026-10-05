using FluentValidation;

namespace HotelManagementSystem.Application.Features.Stays.Queries.GetStayById
{
    public sealed class GetStayByIdQueryValidator : AbstractValidator<GetStayByIdQuery>
    {
        public GetStayByIdQueryValidator()
        {
            RuleFor(x => x.StayId)
                .NotEmpty();
        }
    }
}