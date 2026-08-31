using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagementSystem.Domain.Common.Result.Abstractions
{
    public interface IResult
    {
        IReadOnlyList<Error>? Errors { get; }
        bool IsSuccess { get; }
    }

    public interface IResult<out TValue> : IResult
    {
        TValue Value { get; }
    }
}