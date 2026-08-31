using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result.Abstractions;

namespace HotelManagementSystem.Domain.Common.Result
{
    public sealed class Result
    {
        public static Success Success => default;
        public static Created Created => default;
        public static Updated Updated => default;
        public static Deleted Deleted => default;
    }

    public sealed class Result<TValue> : IResult<TValue>
    {

        private readonly TValue _value;
        private readonly List<Error> _errors;

        public TValue Value => IsSuccess ? _value : throw new InvalidOperationException("The value of a failure result cannot be accessed.");
        public bool IsSuccess { get; }
        public IReadOnlyList<Error> Errors => _errors.AsReadOnly();


        [JsonConstructor]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("For serializer only.", true)]
        public Result(
        TValue? value,
        List<Error>? errors,
        bool isSuccess)
        {
            if (isSuccess)
            {
                _value = value
                    ?? throw new ArgumentNullException(nameof(value));

                _errors = [];
                IsSuccess = true;
            }
            else
            {
                if (errors is null || errors.Count == 0)
                {
                    throw new ArgumentException(
                        "Provide at least one error.",
                        nameof(errors));
                }

                _value = default!;
                _errors = errors;
                IsSuccess = false;
            }
        }


        private Result(TValue value)
        {
            ArgumentNullException.ThrowIfNull(value);

            _value = value;
            _errors = [];
            IsSuccess = true;
        }

        private Result(Error error)
        {
            _value = default!;
            _errors = [error];
            IsSuccess = false;
        }

        private Result(List<Error> errors)
        {
            if (errors is null || errors.Count == 0)
            {
                throw new ArgumentException(
                    "Provide at least one error.",
                    nameof(errors));
            }

            _value = default!;
            _errors = errors;
            IsSuccess = false;

        }

        public Error TopError => IsSuccess ? throw new InvalidOperationException(
                "A successful result has no errors.")
            : _errors[0];

        public TNextValue Match<TNextValue>(
        Func<TValue, TNextValue> onValue,
        Func<List<Error>, TNextValue> onError)
        {
            ArgumentNullException.ThrowIfNull(onValue);
            ArgumentNullException.ThrowIfNull(onError);

            return IsSuccess
                ? onValue(_value)
                : onError(_errors);
        }

        public static implicit operator Result<TValue>(TValue value)
            => new(value);

        public static implicit operator Result<TValue>(Error error)
            => new(error);

        public static implicit operator Result<TValue>(List<Error> errors)
            => new(errors);


    }

    public readonly record struct Success;
    public readonly record struct Created;
    public readonly record struct Updated;
    public readonly record struct Deleted;
}