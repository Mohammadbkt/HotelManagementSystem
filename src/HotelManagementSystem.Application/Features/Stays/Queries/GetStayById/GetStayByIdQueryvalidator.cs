using FluentValidation;

namespace HotelManagementSystem.Application.Features.Stays.Queries.GetStayById
{
    public sealed class GetStayByIdQueryvalidator : AbstractValidator<GetStayByIdQuery>
    {
        public GetStayByIdQueryvalidator()
        {
            RuleFor(x => x.StayId)
                .NotEmpty();
        }
    }
}