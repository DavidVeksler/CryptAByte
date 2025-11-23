using System;
using System.Security.Cryptography;
using CryptAByte.CryptoLibrary.EncryptionLibraries;
using CryptAByte.Domain.Functional;

namespace CryptAByte.CryptoLibrary.CryptoProviders
{
    /// <summary>
    /// Provides asymmetric (RSA) encryption and decryption operations using pure, composable functions.
    /// All functions are stateless and free of side effects, with dependencies explicitly injected.
    /// </summary>
    public sealed class AsymmetricCryptoProvider : ICryptoProvider
    {
        private const int RsaKeySize = 1024;
        private readonly SymmetricCryptoProvider _symmetricProvider;
        private readonly IRandomGenerator _randomGenerator;

        /// <summary>
        /// Creates a new instance with explicit dependencies.
        /// </summary>
        public AsymmetricCryptoProvider(
            SymmetricCryptoProvider symmetricProvider,
            IRandomGenerator randomGenerator)
        {
            _symmetricProvider = symmetricProvider ?? throw new ArgumentNullException(nameof(symmetricProvider));
            _randomGenerator = randomGenerator ?? throw new ArgumentNullException(nameof(randomGenerator));
        }

        /// <summary>
        /// Encrypts data using RSA public key encryption.
        /// Pure function: same inputs always produce same outputs.
        /// </summary>
        public string EncryptWithKey(string secret, string publicKey)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentException("Secret cannot be null or empty", nameof(secret));
            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException("Public key cannot be null or empty", nameof(publicKey));

            return RSAPublicKeyEncryption.EncryptString(secret, RsaKeySize, publicKey);
        }

        /// <summary>
        /// Decrypts data using RSA private key.
        /// Pure function: same inputs always produce same outputs.
        /// </summary>
        public string DecryptWithKey(string secret, string privateKey)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentException("Secret cannot be null or empty", nameof(secret));
            if (string.IsNullOrEmpty(privateKey))
                throw new ArgumentException("Private key cannot be null or empty", nameof(privateKey));

            return RSAPublicKeyEncryption.DecryptString(secret, RsaKeySize, privateKey);
        }

        /// <summary>
        /// Encrypts a message using hybrid encryption (RSA + AES).
        /// Returns an immutable result object instead of using output parameters.
        /// Pure function with explicit randomness dependency.
        /// </summary>
        public AsymmetricEncryptionResult EncryptMessageWithKey(string message, string publicKey)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Message cannot be null or empty", nameof(message));
            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException("Public key cannot be null or empty", nameof(publicKey));

            // Compute hash of original message
            var messageHash = SymmetricCryptoProvider.GetSecureHashForString(message);

            // Generate random AES key
            var encryptionKeyForAES = _randomGenerator.GenerateBase64String(128);

            // Encrypt message with AES
            var encryptedMessage = _symmetricProvider.EncryptWithKey(message, encryptionKeyForAES);

            // Parse encrypted data to extract IV
            var encryptedDataResult = EncryptedData.FromCombinedString(encryptedMessage);
            var initializationVector = encryptedDataResult.Match(
                onSuccess: data => data.InitializationVector,
                onFailure: _ => string.Empty
            );

            // Encrypt AES key with RSA public key
            var encryptedPassword = EncryptWithKey(encryptionKeyForAES, publicKey);

            // Return immutable result
            return new AsymmetricEncryptionResult(
                encryptedMessage,
                encryptedPassword,
                messageHash,
                initializationVector
            );
        }

        /// <summary>
        /// Decrypts a message encrypted with hybrid encryption.
        /// Returns a Result type for explicit error handling instead of assertions.
        /// Pure function: no hidden state or mutations.
        /// </summary>
        public Result<DecryptedData, string> DecryptMessageWithKey(
            string privateKey,
            string messageData,
            string encryptedDecryptionKey,
            string hashOfMessage)
        {
            if (string.IsNullOrEmpty(privateKey))
                return Result.Failure<DecryptedData, string>("Private key cannot be null or empty");
            if (string.IsNullOrEmpty(messageData))
                return Result.Failure<DecryptedData, string>("Message data cannot be null or empty");
            if (string.IsNullOrEmpty(encryptedDecryptionKey))
                return Result.Failure<DecryptedData, string>("Encrypted decryption key cannot be null or empty");
            if (string.IsNullOrEmpty(hashOfMessage))
                return Result.Failure<DecryptedData, string>("Hash of message cannot be null or empty");

            try
            {
                // Decrypt AES key using RSA private key
                var encryptionKey = DecryptWithKey(encryptedDecryptionKey, privateKey);

                if (string.IsNullOrEmpty(encryptionKey))
                    return Result.Failure<DecryptedData, string>("Failed to decrypt encryption key");

                // Decrypt message using AES key
                var decryptedMessage = _symmetricProvider.DecryptWithKey(messageData, encryptionKey);

                // Verify message integrity by comparing hashes
                var computedHash = SymmetricCryptoProvider.GetSecureHashForString(decryptedMessage);
                if (computedHash != hashOfMessage)
                    return Result.Failure<DecryptedData, string>("Message hash verification failed - data may be corrupted");

                // Return immutable result
                return Result.Success<DecryptedData, string>(
                    new DecryptedData(decryptedMessage, encryptionKey, hashOfMessage)
                );
            }
            catch (Exception ex)
            {
                return Result.Failure<DecryptedData, string>($"Decryption failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a new RSA key pair.
        /// Uses system RNG - this is the I/O boundary for key generation.
        /// </summary>
        public static KeyPair GenerateKeys()
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                return new KeyPair(
                    publicKey: rsa.ToXmlString(false),
                    privateKey: rsa.ToXmlString(true)
                );
            }
        }

        #region Legacy Compatibility Methods

        /// <summary>
        /// Legacy method with output parameters - preserved for backward compatibility.
        /// New code should use EncryptMessageWithKey(string, string) instead.
        /// </summary>
        [Obsolete("Use EncryptMessageWithKey(string, string) which returns an immutable result object")]
        public string EncryptMessageWithKey(string message, string publicKey, out string encryptedPassword,
                                            out string hashOfMessage)
        {
            var result = EncryptMessageWithKey(message, publicKey);
            encryptedPassword = result.EncryptedKey;
            hashOfMessage = result.MessageHash;
            return result.EncryptedMessage;
        }

        /// <summary>
        /// Legacy method with output parameters - preserved for backward compatibility.
        /// New code should use DecryptMessageWithKey returning Result type instead.
        /// </summary>
        [Obsolete("Use DecryptMessageWithKey returning Result<DecryptedData, string> for explicit error handling")]
        public string DecryptMessageWithKey(string privateKey, string messageData, string encryptedDecryptionKey,
                                            string hashOfMessage, out string encryptionKey)
        {
            var result = DecryptMessageWithKey(privateKey, messageData, encryptedDecryptionKey, hashOfMessage);

            return result.Match(
                onSuccess: data =>
                {
                    encryptionKey = data.DecryptionKey;
                    return data.PlainText;
                },
                onFailure: error =>
                {
                    encryptionKey = string.Empty;
                    throw new CryptographicException(error);
                }
            );
        }

        #endregion
    }
}