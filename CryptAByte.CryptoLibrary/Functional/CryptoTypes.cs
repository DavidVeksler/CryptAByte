using System;

namespace CryptAByte.CryptoLibrary.Functional
{
    /// <summary>
    /// Immutable result of symmetric encryption operation.
    /// </summary>
    public sealed class EncryptedData
    {
        public string CipherText { get; }
        public string InitializationVector { get; }

        public EncryptedData(string cipherText, string initializationVector)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("Cipher text cannot be null or empty", nameof(cipherText));
            if (string.IsNullOrEmpty(initializationVector))
                throw new ArgumentException("IV cannot be null or empty", nameof(initializationVector));

            CipherText = cipherText;
            InitializationVector = initializationVector;
        }

        /// <summary>
        /// Combines cipher text and IV into a single string for storage.
        /// </summary>
        public string ToCombinedString() => $"{CipherText}:{InitializationVector}";

        /// <summary>
        /// Parses a combined string back into EncryptedData.
        /// </summary>
        public static Result<EncryptedData, string> FromCombinedString(string combined)
        {
            if (string.IsNullOrEmpty(combined))
                return Result.Failure<EncryptedData, string>("Combined string cannot be null or empty");

            var parts = combined.Split(':');
            if (parts.Length != 2)
                return Result.Failure<EncryptedData, string>("Invalid encrypted data format");

            try
            {
                return Result.Success<EncryptedData, string>(
                    new EncryptedData(parts[0], parts[1]));
            }
            catch (Exception ex)
            {
                return Result.Failure<EncryptedData, string>(ex.Message);
            }
        }
    }

    /// <summary>
    /// Immutable result of asymmetric encryption operation.
    /// </summary>
    public sealed class AsymmetricEncryptionResult
    {
        public string EncryptedMessage { get; }
        public string EncryptedKey { get; }
        public string MessageHash { get; }
        public string InitializationVector { get; }

        public AsymmetricEncryptionResult(
            string encryptedMessage,
            string encryptedKey,
            string messageHash,
            string initializationVector)
        {
            if (string.IsNullOrEmpty(encryptedMessage))
                throw new ArgumentException("Encrypted message cannot be null or empty", nameof(encryptedMessage));
            if (string.IsNullOrEmpty(encryptedKey))
                throw new ArgumentException("Encrypted key cannot be null or empty", nameof(encryptedKey));
            if (string.IsNullOrEmpty(messageHash))
                throw new ArgumentException("Message hash cannot be null or empty", nameof(messageHash));
            if (string.IsNullOrEmpty(initializationVector))
                throw new ArgumentException("IV cannot be null or empty", nameof(initializationVector));

            EncryptedMessage = encryptedMessage;
            EncryptedKey = encryptedKey;
            MessageHash = messageHash;
            InitializationVector = initializationVector;
        }
    }

    /// <summary>
    /// Immutable result of decryption operation.
    /// </summary>
    public sealed class DecryptedData
    {
        public string PlainText { get; }
        public string DecryptionKey { get; }
        public string MessageHash { get; }

        public DecryptedData(string plainText, string decryptionKey, string messageHash)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Plain text cannot be null or empty", nameof(plainText));

            PlainText = plainText;
            DecryptionKey = decryptionKey ?? string.Empty;
            MessageHash = messageHash ?? string.Empty;
        }
    }

    /// <summary>
    /// Immutable RSA key pair.
    /// </summary>
    public sealed class KeyPair
    {
        public string PublicKey { get; }
        public string PrivateKey { get; }

        public KeyPair(string publicKey, string privateKey)
        {
            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException("Public key cannot be null or empty", nameof(publicKey));
            if (string.IsNullOrEmpty(privateKey))
                throw new ArgumentException("Private key cannot be null or empty", nameof(privateKey));

            PublicKey = publicKey;
            PrivateKey = privateKey;
        }
    }

    /// <summary>
    /// Immutable passphrase-protected key pair.
    /// </summary>
    public sealed class ProtectedKeyPair
    {
        public string PublicKey { get; }
        public string EncryptedPrivateKey { get; }
        public string Passphrase { get; }

        public ProtectedKeyPair(string publicKey, string encryptedPrivateKey, string passphrase)
        {
            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException("Public key cannot be null or empty", nameof(publicKey));
            if (string.IsNullOrEmpty(encryptedPrivateKey))
                throw new ArgumentException("Encrypted private key cannot be null or empty", nameof(encryptedPrivateKey));
            if (string.IsNullOrEmpty(passphrase))
                throw new ArgumentException("Passphrase cannot be null or empty", nameof(passphrase));

            PublicKey = publicKey;
            EncryptedPrivateKey = encryptedPrivateKey;
            Passphrase = passphrase;
        }
    }

    /// <summary>
    /// Immutable file attachment data.
    /// </summary>
    public sealed class FileAttachment
    {
        public string FileName { get; }
        public byte[] Data { get; }
        public string Base64EncodedZip { get; }

        public FileAttachment(string fileName, byte[] data, string base64EncodedZip)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File name cannot be null or empty", nameof(fileName));
            if (data == null || data.Length == 0)
                throw new ArgumentException("Data cannot be null or empty", nameof(data));
            if (string.IsNullOrEmpty(base64EncodedZip))
                throw new ArgumentException("Base64 encoded zip cannot be null or empty", nameof(base64EncodedZip));

            FileName = fileName;
            Data = data;
            Base64EncodedZip = base64EncodedZip;
        }
    }
}
