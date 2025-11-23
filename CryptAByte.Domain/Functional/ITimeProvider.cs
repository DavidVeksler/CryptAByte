using System;

namespace CryptAByte.Domain.Functional
{
    /// <summary>
    /// Abstraction for providing the current time.
    /// This enables pure functions by making time an explicit dependency rather than a hidden side effect.
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// Gets the current date and time in the local time zone.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets the current date and time in Coordinated Universal Time (UTC).
        /// </summary>
        DateTime UtcNow { get; }
    }

    /// <summary>
    /// Default implementation that returns the system's current time.
    /// </summary>
    public sealed class SystemTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }

    /// <summary>
    /// Fixed time provider for testing and deterministic behavior.
    /// </summary>
    public sealed class FixedTimeProvider : ITimeProvider
    {
        private readonly DateTime _fixedTime;

        public FixedTimeProvider(DateTime fixedTime)
        {
            _fixedTime = fixedTime;
        }

        public DateTime Now => _fixedTime;
        public DateTime UtcNow => _fixedTime.ToUniversalTime();
    }
}
