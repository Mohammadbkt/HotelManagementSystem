using FluentValidation;

namespace HotelManagementSystem.Application.Features.Employees.Queries.GetEmployeeById
{
    public sealed class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
    {
        public GetEmployeeByIdQueryValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty();
        }
    }
}