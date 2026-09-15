using FluentValidation;

namespace HotelManagementSystem.Application.Features.Stays.Queries.GetStays
{
    public sealed class GetStaysQueryValidator : AbstractValidator<GetStaysQuery>
    {
        public GetStaysQueryValidator()
        {
            
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("PageNumber must be at least 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("PageSize must be at least 1.")
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize must not exceed 100.");

           
            RuleFor(x => x.SortDirection)
                .IsInEnum()
                .WithMessage("SortDirection is invalid.");

            RuleFor(x => x)
                .Must(x => !x.CheckInFrom.HasValue || !x.CheckInTo.HasValue || x.CheckInFrom <= x.CheckInTo)
                .WithName(nameof(GetStaysQuery.CheckInFrom))
                .WithMessage("CheckInFrom must be earlier than or equal to CheckInTo.");

            RuleFor(x => x)
                .Must(x => !x.CheckOutFrom.HasValue || !x.CheckOutTo.HasValue || x.CheckOutFrom <= x.CheckOutTo)
                .WithName(nameof(GetStaysQuery.CheckOutFrom))
                .WithMessage("CheckOutFrom must be earlier than or equal to CheckOutTo.");

            RuleFor(x => x)
                .Must(x => !(x.IsActive == true && x.CheckOutFrom.HasValue))
                .When(x => x.IsActive.HasValue)
                .WithName(nameof(GetStaysQuery.IsActive))
                .WithMessage("Cannot filter by IsActive=true together with CheckOutFrom.");
        }

        
    }
}