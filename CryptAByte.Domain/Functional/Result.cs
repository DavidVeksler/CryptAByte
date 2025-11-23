using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptAByte.Domain.Functional
{
    /// <summary>
    /// Represents the result of an operation that can either succeed with a value or fail with an error.
    /// This is a discriminated union that enables explicit error handling without exceptions.
    /// </summary>
    public abstract class Result<TValue, TError>
    {
        private Result() { }

        /// <summary>
        /// Applies a transformation to the success value if present, otherwise propagates the error.
        /// </summary>
        public abstract Result<TResult, TError> Map<TResult>(Func<TValue, TResult> transform);

        /// <summary>
        /// Applies a transformation that returns a Result, enabling chaining of operations.
        /// </summary>
        public abstract Result<TResult, TError> Bind<TResult>(Func<TValue, Result<TResult, TError>> transform);

        /// <summary>
        /// Matches on the result state, applying the appropriate function.
        /// </summary>
        public abstract TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure);

        /// <summary>
        /// Executes an action based on the result state.
        /// </summary>
        public abstract void Match(Action<TValue> onSuccess, Action<TError> onFailure);

        /// <summary>
        /// Returns true if the result is a success.
        /// </summary>
        public abstract bool IsSuccess { get; }

        /// <summary>
        /// Returns true if the result is a failure.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Represents a successful result containing a value.
        /// </summary>
        public sealed class Success : Result<TValue, TError>
        {
            private readonly TValue _value;

            public Success(TValue value)
            {
                _value = value;
            }

            public override bool IsSuccess => true;

            public override Result<TResult, TError> Map<TResult>(Func<TValue, TResult> transform)
            {
                return new Result<TResult, TError>.Success(transform(_value));
            }

            public override Result<TResult, TError> Bind<TResult>(Func<TValue, Result<TResult, TError>> transform)
            {
                return transform(_value);
            }

            public override TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure)
            {
                return onSuccess(_value);
            }

            public override void Match(Action<TValue> onSuccess, Action<TError> onFailure)
            {
                onSuccess(_value);
            }
        }

        /// <summary>
        /// Represents a failed result containing an error.
        /// </summary>
        public sealed class Failure : Result<TValue, TError>
        {
            private readonly TError _error;

            public Failure(TError error)
            {
                _error = error;
            }

            public override bool IsSuccess => false;

            public override Result<TResult, TError> Map<TResult>(Func<TValue, TResult> transform)
            {
                return new Result<TResult, TError>.Failure(_error);
            }

            public override Result<TResult, TError> Bind<TResult>(Func<TValue, Result<TResult, TError>> transform)
            {
                return new Result<TResult, TError>.Failure(_error);
            }

            public override TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onFailure)
            {
                return onFailure(_error);
            }

            public override void Match(Action<TValue> onSuccess, Action<TError> onFailure)
            {
                onFailure(_error);
            }
        }
    }

    /// <summary>
    /// Factory methods for creating Result instances.
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Creates a successful result.
        /// </summary>
        public static Result<TValue, TError> Success<TValue, TError>(TValue value)
        {
            return new Result<TValue, TError>.Success(value);
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        public static Result<TValue, TError> Failure<TValue, TError>(TError error)
        {
            return new Result<TValue, TError>.Failure(error);
        }

        /// <summary>
        /// Converts an enumerable of results into a result of enumerable.
        /// Fails fast on the first error encountered.
        /// </summary>
        public static Result<IEnumerable<TValue>, TError> Sequence<TValue, TError>(
            this IEnumerable<Result<TValue, TError>> results)
        {
            var materializedResults = results.ToList();
            var firstFailure = materializedResults.FirstOrDefault(r => r.IsFailure);

            if (firstFailure != null)
            {
                return firstFailure.Match(
                    onSuccess: _ => throw new InvalidOperationException("Should never happen"),
                    onFailure: error => Result.Failure<IEnumerable<TValue>, TError>(error)
                );
            }

            var values = materializedResults.Select(r => r.Match(
                onSuccess: value => value,
                onFailure: _ => throw new InvalidOperationException("Should never happen")
            ));

            return Result.Success<IEnumerable<TValue>, TError>(values);
        }

        /// <summary>
        /// Executes a function and catches exceptions, converting them to a Result.
        /// </summary>
        public static Result<TValue, Exception> Try<TValue>(Func<TValue> operation)
        {
            try
            {
                return Success<TValue, Exception>(operation());
            }
            catch (Exception ex)
            {
                return Failure<TValue, Exception>(ex);
            }
        }
    }

    /// <summary>
    /// Simplified Result type with string error messages.
    /// </summary>
    public abstract class Result<TValue> : Result<TValue, string>
    {
    }

    /// <summary>
    /// Factory methods for creating simple Result instances with string errors.
    /// </summary>
    public static class SimpleResult
    {
        public static Result<TValue, string> Success<TValue>(TValue value)
        {
            return Result.Success<TValue, string>(value);
        }

        public static Result<TValue, string> Failure<TValue>(string error)
        {
            return Result.Failure<TValue, string>(error);
        }
    }
}
