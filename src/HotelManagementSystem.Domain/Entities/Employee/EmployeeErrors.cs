using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Employee
{
    public static class EmployeeErrors
    {
        public static Error UserIdRequired => Error.Validation(
            "Employee.UserIdRequired",
            "Employee User Id is required");

        public static Error FirstNameRequired => Error.Validation(
            "Employee.FirstNameRequired",
            "Employee First Name is required");

        public static Error LastNameRequired => Error.Validation(
            "Employee.LastNameRequired",
            "Employee Last Name is required");

        public static Error RoleInvalid => Error.Validation(
            "Employee.RoleInvalid",
            "Employee Role is invalid");

        public static Error HotelIdRequired => Error.Validation(
            "Employee.HotelIdRequired",
            "Employee Hotel Id is required");

        public static Error DateOfBirthRequired => Error.Validation(
            "Employee.DateOfBirthRequired",
            "Date Of Birth is required");

        public static Error DateOfBirthInvalid => Error.Validation(
            "Employee.DateOfBirthInvalid",
            "Date Of Birth Invalid");

        public static Error AlreadyLinkedToAccount => Error.Conflict(
            "Employee.AlreadyLinkedToAccount",
            "This employee already has a login account.");

    }
}