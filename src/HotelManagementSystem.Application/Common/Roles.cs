using HotelManagementSystem.Domain.Entities.Employee;

namespace HotelManagementSystem.Application.Common
{
    public static class Roles
    {
        public const string Guest = nameof(Guest);


        private static readonly HashSet<string> StaffRoles =
               Enum.GetNames<EmployeeRole>().ToHashSet();

        public static bool IsStaff(IEnumerable<string> roles) => roles.Any(StaffRoles.Contains);

        public static IEnumerable<string> All =>
            Enum.GetNames<EmployeeRole>().Append(Guest);
    }
}