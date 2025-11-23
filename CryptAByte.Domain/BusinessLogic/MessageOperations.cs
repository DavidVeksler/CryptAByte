using System;
using System.Collections.Generic;
using System.Linq;
using CryptAByte.CryptoLibrary;
using CryptAByte.Domain.Functional;
using CryptAByte.Domain.Models;

namespace CryptAByte.Domain.BusinessLogic
{
    /// <summary>
    /// Pure functional business logic for message operations.
    /// All functions are stateless, composable, and free of side effects.
    /// </summary>
    public static class MessageOperations
    {
        /// <summary>
        /// Decrypts and decompresses a message if it's not a file.
        /// Pure function: explicit data flow, no mutations.
        /// </summary>
        public static Result<ImmutableMessage, string> DecryptAndDecompress(
            ImmutableMessage encryptedMessage,
            string privateKey,
            Func<string, string, string, string, Result<DecryptedData, string>> decryptFunction)
        {
            if (encryptedMessage == null)
                return Result.Failure<ImmutableMessage, string>("Message cannot be null");

            // Decrypt the message
            var decryptResult = MessageDecryption.DecryptMessage(encryptedMessage, privateKey, decryptFunction);

            // If it's not a file, decompress it
            return decryptResult.Bind(decryptedMsg =>
            {
                if (decryptedMsg.IsFile)
                    return Result.Success<ImmutableMessage, string>(decryptedMsg);

                try
                {
                    var decompressedData = GzipCompression.Decompress(decryptedMsg.MessageData);
                    return Result.Success<ImmutableMessage, string>(
                        decryptedMsg.WithMessageData(decompressedData)
                    );
                }
                catch (Exception ex)
                {
                    return Result.Failure<ImmutableMessage, string>($"Decompression failed: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Decrypts and decompresses all messages in a collection.
        /// Pure function: uses map instead of forEach mutations.
        /// </summary>
        public static Result<IReadOnlyList<ImmutableMessage>, string> DecryptAndDecompressAll(
            IEnumerable<ImmutableMessage> encryptedMessages,
            string privateKey,
            Func<string, string, string, string, Result<DecryptedData, string>> decryptFunction)
        {
            if (encryptedMessages == null)
                return Result.Failure<IReadOnlyList<ImmutableMessage>, string>("Messages cannot be null");

            var messagesList = encryptedMessages.ToList();
            var results = messagesList
                .Select(msg => DecryptAndDecompress(msg, privateKey, decryptFunction))
                .ToList();

            return results.Sequence().Map(messages => (IReadOnlyList<ImmutableMessage>)messages.ToList().AsReadOnly());
        }

        /// <summary>
        /// Filters messages that should be deleted based on crypto key settings.
        /// Pure function: returns new collection without mutating inputs.
        /// </summary>
        public static IReadOnlyList<ImmutableMessage> GetMessagesToDelete(
            ImmutableCryptoKey cryptoKey,
            IEnumerable<ImmutableMessage> messages)
        {
            if (cryptoKey == null || messages == null)
                return new List<ImmutableMessage>().AsReadOnly();

            return cryptoKey.DeleteMessagesAfterReading
                ? messages.ToList().AsReadOnly()
                : new List<ImmutableMessage>().AsReadOnly();
        }

        /// <summary>
        /// Determines if the crypto key should be deleted based on its settings.
        /// Pure function: no side effects, explicit logic.
        /// </summary>
        public static bool ShouldDeleteKey(ImmutableCryptoKey cryptoKey)
        {
            return cryptoKey?.DeleteKeyAfterReading ?? false;
        }

        /// <summary>
        /// Checks if a crypto key is valid for reading messages at the current time.
        /// Pure function: time is explicitly passed as parameter.
        /// </summary>
        public static Result<ImmutableCryptoKey, string> ValidateKeyForReading(
            ImmutableCryptoKey cryptoKey,
            DateTime currentTime)
        {
            if (cryptoKey == null)
                return Result.Failure<ImmutableCryptoKey, string>("Crypto key not found");

            if (!cryptoKey.IsReleased(currentTime))
                return Result.Failure<ImmutableCryptoKey, string>(
                    $"Key is not yet released. Release date: {cryptoKey.ReleaseDate}");

            if (cryptoKey.LockDate.HasValue && cryptoKey.LockDate.Value < currentTime)
                return Result.Failure<ImmutableCryptoKey, string>(
                    $"Key is locked. Lock date: {cryptoKey.LockDate}");

            return Result.Success<ImmutableCryptoKey, string>(cryptoKey);
        }

        /// <summary>
        /// Validates and decrypts a private key if it's encrypted with a passphrase.
        /// Pure function: explicit error handling with Result type.
        /// </summary>
        public static Result<string, string> DecryptPrivateKey(
            ImmutableCryptoKey cryptoKey,
            string passphrase,
            Func<string, string, string> decryptFunction,
            Func<string, string> hashFunction)
        {
            if (cryptoKey == null)
                return Result.Failure<string, string>("Crypto key cannot be null");

            if (!cryptoKey.IsPrivateKeyEncrypted)
                return Result.Success<string, string>(cryptoKey.PrivateKey);

            if (string.IsNullOrEmpty(passphrase))
                return Result.Failure<string, string>("Passphrase is required for encrypted private key");

            try
            {
                var decryptedPrivateKey = decryptFunction(cryptoKey.PrivateKey, passphrase);

                // Verify the decrypted key matches the stored hash
                if (!string.IsNullOrEmpty(cryptoKey.PrivateKeyHash))
                {
                    var computedHash = hashFunction(decryptedPrivateKey);
                    if (computedHash != cryptoKey.PrivateKeyHash)
                        return Result.Failure<string, string>("Invalid passphrase - hash verification failed");
                }

                return Result.Success<string, string>(decryptedPrivateKey);
            }
            catch (Exception ex)
            {
                return Result.Failure<string, string>($"Private key decryption failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Compresses and encrypts a message for storage.
        /// Pure function: returns encrypted message data without mutations.
        /// </summary>
        public static Result<string, string> CompressAndEncryptMessage(
            string plaintext,
            Func<string, string, string> encryptFunction,
            string encryptionKey)
        {
            if (string.IsNullOrEmpty(plaintext))
                return Result.Failure<string, string>("Plaintext cannot be null or empty");
            if (encryptFunction == null)
                return Result.Failure<string, string>("Encrypt function cannot be null");
            if (string.IsNullOrEmpty(encryptionKey))
                return Result.Failure<string, string>("Encryption key cannot be null or empty");

            try
            {
                var compressed = GzipCompression.Compress(plaintext);
                var encrypted = encryptFunction(compressed, encryptionKey);
                return Result.Success<string, string>(encrypted);
            }
            catch (Exception ex)
            {
                return Result.Failure<string, string>($"Compression and encryption failed: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Pure functional business logic for crypto key operations.
    /// </summary>
    public static class CryptoKeyOperations
    {
        /// <summary>
        /// Creates a new crypto key with generated RSA keys.
        /// This function has I/O side effect (RNG) and should be called at the I/O boundary.
        /// </summary>
        public static ImmutableCryptoKey CreateWithGeneratedKeys(
            string keyToken,
            DateTime requestDate,
            DateTime releaseDate,
            Func<KeyPair> generateKeysFunction)
        {
            if (string.IsNullOrEmpty(keyToken))
                throw new ArgumentException("Key token cannot be null or empty", nameof(keyToken));
            if (generateKeysFunction == null)
                throw new ArgumentNullException(nameof(generateKeysFunction));

            var keyPair = generateKeysFunction();

            return new ImmutableCryptoKey(
                keyId: 0, // Will be set by EF on insert
                keyToken: keyToken,
                publicKey: keyPair.PublicKey,
                privateKey: keyPair.PrivateKey,
                privateKeyHash: null,
                isPrivateKeyEncrypted: false,
                isPublicKeyOnly: false,
                requestDate: requestDate,
                releaseDate: releaseDate,
                lockDate: null,
                deleteMessagesAfterReading: false,
                deleteKeyAfterReading: false
            );
        }

        /// <summary>
        /// Creates a new crypto key with passphrase-protected keys.
        /// Pure function with explicit dependencies.
        /// </summary>
        public static ImmutableCryptoKey CreateWithPassphraseProtectedKeys(
            string keyToken,
            DateTime requestDate,
            DateTime releaseDate,
            string passphrase,
            Func<KeyPair> generateKeysFunction,
            Func<string, string, string> encryptFunction,
            Func<string, string> hashFunction)
        {
            if (string.IsNullOrEmpty(keyToken))
                throw new ArgumentException("Key token cannot be null or empty", nameof(keyToken));
            if (string.IsNullOrEmpty(passphrase))
                throw new ArgumentException("Passphrase cannot be null or empty", nameof(passphrase));
            if (generateKeysFunction == null)
                throw new ArgumentNullException(nameof(generateKeysFunction));
            if (encryptFunction == null)
                throw new ArgumentNullException(nameof(encryptFunction));
            if (hashFunction == null)
                throw new ArgumentNullException(nameof(hashFunction));

            var keyPair = generateKeysFunction();
            var encryptedPrivateKey = encryptFunction(keyPair.PrivateKey, passphrase);
            var privateKeyHash = hashFunction(keyPair.PrivateKey);

            return new ImmutableCryptoKey(
                keyId: 0, // Will be set by EF on insert
                keyToken: keyToken,
                publicKey: keyPair.PublicKey,
                privateKey: encryptedPrivateKey,
                privateKeyHash: privateKeyHash,
                isPrivateKeyEncrypted: true,
                isPublicKeyOnly: false,
                requestDate: requestDate,
                releaseDate: releaseDate,
                lockDate: null,
                deleteMessagesAfterReading: false,
                deleteKeyAfterReading: false
            );
        }

        /// <summary>
        /// Removes the private key from a crypto key, making it public-key-only.
        /// Pure function: returns new instance without mutating the original.
        /// </summary>
        public static ImmutableCryptoKey MakePublicKeyOnly(ImmutableCryptoKey cryptoKey)
        {
            if (cryptoKey == null)
                throw new ArgumentNullException(nameof(cryptoKey));

            return cryptoKey.WithoutPrivateKey();
        }
    }
}
