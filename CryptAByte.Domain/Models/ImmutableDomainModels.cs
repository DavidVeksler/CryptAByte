using System;
using System.Collections.Generic;
using System.Linq;
using CryptAByte.Domain.Functional;
using CryptAByte.Domain.KeyManager;

namespace CryptAByte.Domain.Models
{
    /// <summary>
    /// Immutable representation of a crypto key for business logic.
    /// This separates domain logic from persistence concerns.
    /// </summary>
    public sealed class ImmutableCryptoKey
    {
        public int KeyId { get; }
        public string KeyToken { get; }
        public string PublicKey { get; }
        public string PrivateKey { get; }
        public string PrivateKeyHash { get; }
        public bool IsPrivateKeyEncrypted { get; }
        public bool IsPublicKeyOnly { get; }
        public DateTime RequestDate { get; }
        public DateTime ReleaseDate { get; }
        public DateTime? LockDate { get; }
        public bool DeleteMessagesAfterReading { get; }
        public bool DeleteKeyAfterReading { get; }
        public IReadOnlyList<ImmutableMessage> Messages { get; }
        public IReadOnlyList<ImmutableNotification> Notifications { get; }

        public ImmutableCryptoKey(
            int keyId,
            string keyToken,
            string publicKey,
            string privateKey,
            string privateKeyHash,
            bool isPrivateKeyEncrypted,
            bool isPublicKeyOnly,
            DateTime requestDate,
            DateTime releaseDate,
            DateTime? lockDate,
            bool deleteMessagesAfterReading,
            bool deleteKeyAfterReading,
            IEnumerable<ImmutableMessage> messages = null,
            IEnumerable<ImmutableNotification> notifications = null)
        {
            KeyId = keyId;
            KeyToken = keyToken ?? throw new ArgumentNullException(nameof(keyToken));
            PublicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
            PrivateKey = privateKey; // Can be null for public-key-only scenarios
            PrivateKeyHash = privateKeyHash;
            IsPrivateKeyEncrypted = isPrivateKeyEncrypted;
            IsPublicKeyOnly = isPublicKeyOnly;
            RequestDate = requestDate;
            ReleaseDate = releaseDate;
            LockDate = lockDate;
            DeleteMessagesAfterReading = deleteMessagesAfterReading;
            DeleteKeyAfterReading = deleteKeyAfterReading;
            Messages = (messages ?? Enumerable.Empty<ImmutableMessage>()).ToList().AsReadOnly();
            Notifications = (notifications ?? Enumerable.Empty<ImmutableNotification>()).ToList().AsReadOnly();
        }

        /// <summary>
        /// Pure function to determine if the key is released based on current time.
        /// </summary>
        public bool IsReleased(DateTime currentTime) => ReleaseDate < currentTime;

        /// <summary>
        /// Pure function to get the private key only if released.
        /// </summary>
        public Option<string> GetPrivateKeyIfReleased(DateTime currentTime)
        {
            return IsReleased(currentTime) && !string.IsNullOrEmpty(PrivateKey)
                ? Option.Some(PrivateKey)
                : Option.None<string>();
        }

        /// <summary>
        /// Creates a copy with updated messages.
        /// Pure function: returns new instance without mutating the original.
        /// </summary>
        public ImmutableCryptoKey WithMessages(IEnumerable<ImmutableMessage> newMessages)
        {
            return new ImmutableCryptoKey(
                KeyId, KeyToken, PublicKey, PrivateKey, PrivateKeyHash,
                IsPrivateKeyEncrypted, IsPublicKeyOnly, RequestDate, ReleaseDate, LockDate,
                DeleteMessagesAfterReading, DeleteKeyAfterReading, newMessages, Notifications
            );
        }

        /// <summary>
        /// Creates a copy with the private key removed.
        /// Pure function: returns new instance without mutating the original.
        /// </summary>
        public ImmutableCryptoKey WithoutPrivateKey()
        {
            return new ImmutableCryptoKey(
                KeyId, KeyToken, PublicKey, null, PrivateKeyHash,
                IsPrivateKeyEncrypted, true, RequestDate, ReleaseDate, LockDate,
                DeleteMessagesAfterReading, DeleteKeyAfterReading, Messages, Notifications
            );
        }

        /// <summary>
        /// Converts from EF entity to immutable domain model.
        /// Pure transformation function.
        /// </summary>
        public static ImmutableCryptoKey FromEntity(CryptoKey entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new ImmutableCryptoKey(
                keyId: entity.KeyId,
                keyToken: entity.KeyToken,
                publicKey: entity.PublicKey,
                privateKey: entity.PrivateKey,
                privateKeyHash: entity.PrivateKeyHash,
                isPrivateKeyEncrypted: entity.IsPrivateKeyEncrypted,
                isPublicKeyOnly: entity.IsPublicKeyOnly,
                requestDate: entity.RequestDate,
                releaseDate: entity.ReleaseDate,
                lockDate: entity.LockDate,
                deleteMessagesAfterReading: entity.DeleteMessagesAfterReading,
                deleteKeyAfterReading: entity.DeleteKeyAfterReading,
                messages: entity.Messages?.Select(ImmutableMessage.FromEntity),
                notifications: entity.Notifications?.Select(ImmutableNotification.FromEntity)
            );
        }

        /// <summary>
        /// Converts to EF entity for persistence.
        /// This is the I/O boundary transformation.
        /// </summary>
        public CryptoKey ToEntity()
        {
            return new CryptoKey
            {
                KeyId = KeyId,
                KeyToken = KeyToken,
                PublicKey = PublicKey,
                PrivateKey = PrivateKey,
                PrivateKeyHash = PrivateKeyHash,
                IsPrivateKeyEncrypted = IsPrivateKeyEncrypted,
                IsPublicKeyOnly = IsPublicKeyOnly,
                RequestDate = RequestDate,
                ReleaseDate = ReleaseDate,
                LockDate = LockDate,
                DeleteMessagesAfterReading = DeleteMessagesAfterReading,
                DeleteKeyAfterReading = DeleteKeyAfterReading,
            };
        }
    }

    /// <summary>
    /// Immutable representation of a message for business logic.
    /// </summary>
    public sealed class ImmutableMessage
    {
        public int MessageId { get; }
        public string EncryptionKey { get; }
        public string MessageData { get; }
        public DateTime Created { get; }
        public string MessageHash { get; }
        public bool IsFile { get; }
        public string TemporaryDownloadId { get; }

        public ImmutableMessage(
            int messageId,
            string encryptionKey,
            string messageData,
            DateTime created,
            string messageHash,
            bool isFile,
            string temporaryDownloadId = null)
        {
            MessageId = messageId;
            EncryptionKey = encryptionKey ?? string.Empty;
            MessageData = messageData ?? throw new ArgumentNullException(nameof(messageData));
            Created = created;
            MessageHash = messageHash ?? string.Empty;
            IsFile = isFile;
            TemporaryDownloadId = temporaryDownloadId;
        }

        /// <summary>
        /// Creates a copy with updated message data.
        /// Pure function: returns new instance without mutating the original.
        /// </summary>
        public ImmutableMessage WithMessageData(string newMessageData)
        {
            return new ImmutableMessage(
                MessageId, EncryptionKey, newMessageData, Created,
                MessageHash, IsFile, TemporaryDownloadId
            );
        }

        /// <summary>
        /// Creates a copy with updated encryption key.
        /// Pure function: returns new instance without mutating the original.
        /// </summary>
        public ImmutableMessage WithEncryptionKey(string newEncryptionKey)
        {
            return new ImmutableMessage(
                MessageId, newEncryptionKey, MessageData, Created,
                MessageHash, IsFile, TemporaryDownloadId
            );
        }

        /// <summary>
        /// Creates a copy with temporary download ID.
        /// Pure function: returns new instance without mutating the original.
        /// </summary>
        public ImmutableMessage WithTemporaryDownloadId(string downloadId)
        {
            return new ImmutableMessage(
                MessageId, EncryptionKey, MessageData, Created,
                MessageHash, IsFile, downloadId
            );
        }

        /// <summary>
        /// Converts from EF entity to immutable domain model.
        /// Pure transformation function.
        /// </summary>
        public static ImmutableMessage FromEntity(Message entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new ImmutableMessage(
                messageId: entity.MessageId,
                encryptionKey: entity.EncryptionKey,
                messageData: entity.MessageData,
                created: entity.Created,
                messageHash: entity.MessageHash,
                isFile: entity.IsFile,
                temporaryDownloadId: entity.TemporaryDownloadId
            );
        }

        /// <summary>
        /// Converts to EF entity for persistence.
        /// This is the I/O boundary transformation.
        /// </summary>
        public Message ToEntity()
        {
            return new Message
            {
                MessageId = MessageId,
                EncryptionKey = EncryptionKey,
                MessageData = MessageData,
                Created = Created,
                MessageHash = MessageHash,
                IsFile = IsFile,
                TemporaryDownloadId = TemporaryDownloadId
            };
        }
    }

    /// <summary>
    /// Immutable representation of a notification for business logic.
    /// </summary>
    public sealed class ImmutableNotification
    {
        public int RequestId { get; }
        public string Email { get; }
        public bool WasNotified { get; }

        public ImmutableNotification(int requestId, string email, bool wasNotified)
        {
            RequestId = requestId;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            WasNotified = wasNotified;
        }

        /// <summary>
        /// Creates a copy marked as notified.
        /// Pure function: returns new instance without mutating the original.
        /// </summary>
        public ImmutableNotification MarkAsNotified()
        {
            return new ImmutableNotification(RequestId, Email, true);
        }

        /// <summary>
        /// Converts from EF entity to immutable domain model.
        /// Pure transformation function.
        /// </summary>
        public static ImmutableNotification FromEntity(Notification entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new ImmutableNotification(
                requestId: entity.RequestId,
                email: entity.Email,
                wasNotified: entity.WasNotified
            );
        }

        /// <summary>
        /// Converts to EF entity for persistence.
        /// This is the I/O boundary transformation.
        /// </summary>
        public Notification ToEntity()
        {
            return new Notification
            {
                RequestId = RequestId,
                Email = Email,
                WasNotified = WasNotified
            };
        }
    }

    /// <summary>
    /// Pure functions for decrypting messages without mutations.
    /// </summary>
    public static class MessageDecryption
    {
        /// <summary>
        /// Decrypts a single message and returns a new immutable message with decrypted data.
        /// Pure function: no mutations, explicit data flow.
        /// </summary>
        public static Result<ImmutableMessage, string> DecryptMessage(
            ImmutableMessage encryptedMessage,
            string privateKey,
            Func<string, string, string, string, Result<DecryptedData, string>> decryptFunction)
        {
            if (encryptedMessage == null)
                return Result.Failure<ImmutableMessage, string>("Message cannot be null");
            if (string.IsNullOrEmpty(privateKey))
                return Result.Failure<ImmutableMessage, string>("Private key cannot be null or empty");
            if (decryptFunction == null)
                return Result.Failure<ImmutableMessage, string>("Decrypt function cannot be null");

            return decryptFunction(
                privateKey,
                encryptedMessage.MessageData,
                encryptedMessage.EncryptionKey,
                encryptedMessage.MessageHash
            ).Map(decryptedData =>
                encryptedMessage
                    .WithMessageData(decryptedData.PlainText)
                    .WithEncryptionKey(decryptedData.DecryptionKey)
            );
        }

        /// <summary>
        /// Decrypts all messages and returns a new list of immutable messages.
        /// Pure function: uses map instead of forEach mutations.
        /// </summary>
        public static Result<IReadOnlyList<ImmutableMessage>, string> DecryptMessages(
            IEnumerable<ImmutableMessage> encryptedMessages,
            string privateKey,
            Func<string, string, string, string, Result<DecryptedData, string>> decryptFunction)
        {
            if (encryptedMessages == null)
                return Result.Failure<IReadOnlyList<ImmutableMessage>, string>("Messages cannot be null");

            var messagesList = encryptedMessages.ToList();
            var decryptedResults = messagesList
                .Select(msg => DecryptMessage(msg, privateKey, decryptFunction))
                .ToList();

            // Use Sequence to convert List<Result<T>> to Result<List<T>>
            return decryptedResults
                .Sequence()
                .Map(messages => (IReadOnlyList<ImmutableMessage>)messages.ToList().AsReadOnly());
        }
    }
}
