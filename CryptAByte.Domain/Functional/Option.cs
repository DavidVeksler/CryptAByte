using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptAByte.Domain.Functional
{
    /// <summary>
    /// Represents an optional value that may or may not be present.
    /// This eliminates null reference exceptions by making optionality explicit.
    /// </summary>
    public abstract class Option<T>
    {
        private Option() { }

        /// <summary>
        /// Applies a transformation to the value if present, otherwise returns None.
        /// </summary>
        public abstract Option<TResult> Map<TResult>(Func<T, TResult> transform);

        /// <summary>
        /// Applies a transformation that returns an Option, enabling chaining of operations.
        /// </summary>
        public abstract Option<TResult> Bind<TResult>(Func<T, Option<TResult>> transform);

        /// <summary>
        /// Matches on the option state, applying the appropriate function.
        /// </summary>
        public abstract TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone);

        /// <summary>
        /// Executes an action based on the option state.
        /// </summary>
        public abstract void Match(Action<T> onSome, Action onNone);

        /// <summary>
        /// Returns the value if present, otherwise returns the provided default value.
        /// </summary>
        public abstract T GetValueOrDefault(T defaultValue);

        /// <summary>
        /// Returns the value if present, otherwise computes and returns a default value.
        /// </summary>
        public abstract T GetValueOrDefault(Func<T> defaultValueFactory);

        /// <summary>
        /// Returns true if a value is present.
        /// </summary>
        public abstract bool IsSome { get; }

        /// <summary>
        /// Returns true if no value is present.
        /// </summary>
        public bool IsNone => !IsSome;

        /// <summary>
        /// Filters the option based on a predicate.
        /// </summary>
        public abstract Option<T> Where(Func<T, bool> predicate);

        /// <summary>
        /// Represents an option containing a value.
        /// </summary>
        public sealed class Some : Option<T>
        {
            private readonly T _value;

            public Some(T value)
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Cannot create Some with null value");
                _value = value;
            }

            public override bool IsSome => true;

            public override Option<TResult> Map<TResult>(Func<T, TResult> transform)
            {
                return Option.Some(transform(_value));
            }

            public override Option<TResult> Bind<TResult>(Func<T, Option<TResult>> transform)
            {
                return transform(_value);
            }

            public override TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
            {
                return onSome(_value);
            }

            public override void Match(Action<T> onSome, Action onNone)
            {
                onSome(_value);
            }

            public override T GetValueOrDefault(T defaultValue)
            {
                return _value;
            }

            public override T GetValueOrDefault(Func<T> defaultValueFactory)
            {
                return _value;
            }

            public override Option<T> Where(Func<T, bool> predicate)
            {
                return predicate(_value) ? this : Option.None<T>();
            }
        }

        /// <summary>
        /// Represents an option with no value.
        /// </summary>
        public sealed class None : Option<T>
        {
            public override bool IsSome => false;

            public override Option<TResult> Map<TResult>(Func<T, TResult> transform)
            {
                return Option.None<TResult>();
            }

            public override Option<TResult> Bind<TResult>(Func<T, Option<TResult>> transform)
            {
                return Option.None<TResult>();
            }

            public override TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
            {
                return onNone();
            }

            public override void Match(Action<T> onSome, Action onNone)
            {
                onNone();
            }

            public override T GetValueOrDefault(T defaultValue)
            {
                return defaultValue;
            }

            public override T GetValueOrDefault(Func<T> defaultValueFactory)
            {
                return defaultValueFactory();
            }

            public override Option<T> Where(Func<T, bool> predicate)
            {
                return this;
            }
        }
    }

    /// <summary>
    /// Factory methods for creating Option instances.
    /// </summary>
    public static class Option
    {
        /// <summary>
        /// Creates an option containing a value.
        /// </summary>
        public static Option<T> Some<T>(T value)
        {
            return new Option<T>.Some(value);
        }

        /// <summary>
        /// Creates an option with no value.
        /// </summary>
        public static Option<T> None<T>()
        {
            return new Option<T>.None();
        }

        /// <summary>
        /// Creates an option from a nullable value.
        /// Returns Some if the value is not null, otherwise None.
        /// </summary>
        public static Option<T> FromNullable<T>(T value) where T : class
        {
            return value != null ? Some(value) : None<T>();
        }

        /// <summary>
        /// Creates an option from a nullable struct.
        /// Returns Some if the value has a value, otherwise None.
        /// </summary>
        public static Option<T> FromNullable<T>(T? value) where T : struct
        {
            return value.HasValue ? Some(value.Value) : None<T>();
        }

        /// <summary>
        /// Converts an enumerable of options into an option of enumerable.
        /// Returns Some with all values if all options are Some, otherwise None.
        /// </summary>
        public static Option<IEnumerable<T>> Sequence<T>(this IEnumerable<Option<T>> options)
        {
            var materializedOptions = options.ToList();

            if (materializedOptions.Any(o => o.IsNone))
            {
                return None<IEnumerable<T>>();
            }

            var values = materializedOptions.Select(o => o.Match(
                onSome: value => value,
                onNone: () => throw new InvalidOperationException("Should never happen")
            ));

            return Some(values);
        }

        /// <summary>
        /// Tries to perform an operation, returning Some with the result if successful, otherwise None.
        /// </summary>
        public static Option<T> Try<T>(Func<T> operation)
        {
            try
            {
                return Some(operation());
            }
            catch
            {
                return None<T>();
            }
        }
    }
}
