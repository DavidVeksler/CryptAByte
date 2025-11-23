using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptAByte.CryptoLibrary.EncryptionLibraries;

namespace CryptAByte.CryptoLibrary.CryptoProviders
{
    /// <summary>
    /// Provides symmetric (AES) encryption and decryption operations using pure, composable functions.
    /// All functions are stateless and free of side effects.
    /// </summary>
    public sealed class SymmetricCryptoProvider : ICryptoProvider
    {
        private const string DefaultEncryptionSalt = "CryptAByte";
        private readonly string _encryptionSalt;

        /// <summary>
        /// Creates a new instance with the default encryption salt.
        /// </summary>
        public SymmetricCryptoProvider() : this(DefaultEncryptionSalt)
        {
        }

        /// <summary>
        /// Creates a new instance with a custom encryption salt.
        /// </summary>
        public SymmetricCryptoProvider(string encryptionSalt)
        {
            _encryptionSalt = encryptionSalt ?? throw new ArgumentNullException(nameof(encryptionSalt));
        }

        /// <summary>
        /// Encrypts plaintext using AES-256 encryption.
        /// Pure function: same inputs always produce same outputs (given same salt).
        /// </summary>
        public string EncryptWithKey(string plaintext, string encryptionKey)
        {
            if (string.IsNullOrEmpty(plaintext))
                throw new ArgumentException("Plaintext cannot be null or empty", nameof(plaintext));
            if (string.IsNullOrEmpty(encryptionKey))
                throw new ArgumentException("Encryption key cannot be null or empty", nameof(encryptionKey));

            return AESEncryption.Encrypt<AesManaged>(plaintext, encryptionKey, _encryptionSalt);
        }

        /// <summary>
        /// Decrypts ciphertext using AES-256 decryption.
        /// Pure function: same inputs always produce same outputs (given same salt).
        /// </summary>
        public string DecryptWithKey(string ciphertext, string decryptionKey)
        {
            if (string.IsNullOrEmpty(ciphertext))
                throw new ArgumentException("Ciphertext cannot be null or empty", nameof(ciphertext));
            if (string.IsNullOrEmpty(decryptionKey))
                throw new ArgumentException("Decryption key cannot be null or empty", nameof(decryptionKey));

            return AESEncryption.Decrypt<AesManaged>(ciphertext, decryptionKey, _encryptionSalt);
        }

        /// <summary>
        /// Computes a SHA256 hash of the input text.
        /// Pure function: same input always produces same hash.
        /// </summary>
        public static string GetSecureHashForString(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text cannot be null or empty", nameof(text));

            byte[] messageBytes = Encoding.Unicode.GetBytes(text);

            using (var hashAlgorithm = SHA256.Create())
            {
                byte[] hashValue = hashAlgorithm.ComputeHash(messageBytes);
                return string.Concat(hashValue.Select(b => b.ToString("x2")));
            }
        }

        /// <summary>
        /// Legacy static method for generating random key phrases.
        /// This method has hidden side effects (uses system RNG).
        /// Use IRandomGenerator dependency injection in new code instead.
        /// </summary>
        [Obsolete("Use IRandomGenerator.GenerateBase64String() for testable random generation")]
        public static string GenerateKeyPhrase(int sizeInBytes = 128)
        {
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[sizeInBytes];
                randomNumberGenerator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }

    /// <summary>
    /// Pure functional utilities for cryptographic operations.
    /// All functions are stateless with no side effects.
    /// </summary>
    public static class CryptoFunctions
    {
        /// <summary>
        /// Computes SHA256 hash of the input text.
        /// Pure function: deterministic and referentially transparent.
        /// </summary>
        public static string ComputeHash(string text)
        {
            return SymmetricCryptoProvider.GetSecureHashForString(text);
        }

        /// <summary>
        /// Verifies that a text matches a given hash.
        /// Pure function: no side effects.
        /// </summary>
        public static bool VerifyHash(string text, string expectedHash)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(expectedHash))
                return false;

            var actualHash = ComputeHash(text);
            return string.Equals(actualHash, expectedHash, StringComparison.Ordinal);
        }
    }
}