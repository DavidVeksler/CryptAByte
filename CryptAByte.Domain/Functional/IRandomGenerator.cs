using System;
using System.Security.Cryptography;

namespace CryptAByte.Domain.Functional
{
    /// <summary>
    /// Abstraction for generating random data.
    /// This enables pure functions by making randomness an explicit dependency.
    /// </summary>
    public interface IRandomGenerator
    {
        /// <summary>
        /// Generates random bytes of the specified length.
        /// </summary>
        byte[] GenerateBytes(int length);

        /// <summary>
        /// Generates a random Base64-encoded string.
        /// </summary>
        string GenerateBase64String(int sizeInBytes);
    }

    /// <summary>
    /// Cryptographically secure random generator using RNGCryptoServiceProvider.
    /// </summary>
    public sealed class CryptoRandomGenerator : IRandomGenerator
    {
        private readonly RandomNumberGenerator _rng;

        public CryptoRandomGenerator()
        {
            _rng = RandomNumberGenerator.Create();
        }

        public byte[] GenerateBytes(int length)
        {
            if (length <= 0)
                throw new ArgumentException("Length must be positive", nameof(length));

            var bytes = new byte[length];
            _rng.GetBytes(bytes);
            return bytes;
        }

        public string GenerateBase64String(int sizeInBytes)
        {
            var bytes = GenerateBytes(sizeInBytes);
            return Convert.ToBase64String(bytes);
        }
    }

    /// <summary>
    /// Deterministic random generator for testing.
    /// </summary>
    public sealed class DeterministicRandomGenerator : IRandomGenerator
    {
        private readonly byte _fillByte;

        public DeterministicRandomGenerator(byte fillByte = 0x42)
        {
            _fillByte = fillByte;
        }

        public byte[] GenerateBytes(int length)
        {
            if (length <= 0)
                throw new ArgumentException("Length must be positive", nameof(length));

            var bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = (byte)((_fillByte + i) % 256);
            }
            return bytes;
        }

        public string GenerateBase64String(int sizeInBytes)
        {
            var bytes = GenerateBytes(sizeInBytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
