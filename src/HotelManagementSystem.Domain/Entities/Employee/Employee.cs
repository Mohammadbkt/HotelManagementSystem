using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;

namespace HotelManagementSystem.Domain.Entities.Employee
{
    public sealed class Employee : SoftDeletableEntity
    {
        public Guid HotelId { get; private set; }
        public string FirstName { get; private set; } = default!;
        public string LastName { get; private set; } = default!;
        public PhoneNumber? PhoneNumber { get; private set; }
        public EmailAddress? EmailAddress { get; private set; }
        public EmployeeRole Role { get; private set; }
        public string FullName => $"{FirstName} {LastName}";

        private Employee()
        {
        }

        private Employee(Guid id, Guid hotelId, string firstName, string lastName, PhoneNumber? phoneNumber, EmailAddress emailAddress, EmployeeRole role)
            : base(id)
        {
            HotelId = hotelId;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            EmailAddress = emailAddress;
            Role = role;
        }

        public static Result<Employee> Create(Guid hotelId, string firstName, string lastName, PhoneNumber? phoneNumber, EmailAddress emailAddress, EmployeeRole role)
        {
            if (hotelId == Guid.Empty)
            {
                return Result<Employee>.Failure(EmployeeErrors.HotelIdRequired);
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                return Result<Employee>.Failure(EmployeeErrors.FirstNameRequired);
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                return Result<Employee>.Failure(EmployeeErrors.LastNameRequired);
            }

            if (!Enum.IsDefined(role))
            {
                return Result<Employee>.Failure(EmployeeErrors.RoleInvalid);
            }

            return Result<Employee>.Success(new Employee(Guid.NewGuid(), hotelId, firstName.Trim(), lastName.Trim(), phoneNumber, emailAddress, role));
        }

        public Result ChangeRole(EmployeeRole role)
        {
            if (!Enum.IsDefined(role))
                return Result.Failure(EmployeeErrors.RoleInvalid);

            Role = role;
            return Result.Success();
        }

        public Result UpdateContactInfo(PhoneNumber? phoneNumber, EmailAddress? emailAddress)
        {
            PhoneNumber = phoneNumber;
            EmailAddress = emailAddress;
            return Result.Success();
        }
    }
}