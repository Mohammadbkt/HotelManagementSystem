using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Employee
{
    public static class EmployeeErrors
    {
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
    }
}