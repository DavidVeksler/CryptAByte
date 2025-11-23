using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Xml.Serialization;
using CryptAByte.CryptoLibrary.CryptoProviders;
using CryptAByte.Domain.DataContext;

namespace CryptAByte.Domain.KeyManager
{
    public sealed class CryptoKey
    {
        public CryptoKey()
        {
        }

        public static CryptoKey CreateWithGeneratedKeys(DateTime? releaseDate = null)
        {
            var keyPair = AsymmetricCryptoProvider.GenerateKeys();
            DateTime now = DateTime.UtcNow;

            return new CryptoKey
            {
                RequestDate = now,
                ReleaseDate = releaseDate ?? now,
                KeyToken = UniqueIdGenerator.GetUniqueId(),
                PrivateKey = keyPair.PrivateKey,
                PublicKey = keyPair.PublicKey,
                IsPrivateKeyEncrypted = false,
                IsPublicKeyOnly = false,
                Notifications = new EntityCollection<Notification>(),
                Messages = new EntityCollection<Message>(),
            };
        }

        public static CryptoKey CreateWithPassphraseProtectedKeys(string passphrase)
        {
            if (string.IsNullOrWhiteSpace(passphrase))
                throw new ArgumentException("Passphrase cannot be empty.", nameof(passphrase));

            var keyPair = AsymmetricCryptoProvider.GenerateKeys();
            var symmetricProvider = new SymmetricCryptoProvider();

            return new CryptoKey
            {
                RequestDate = DateTime.UtcNow,
                ReleaseDate = DateTime.UtcNow,
                KeyToken = UniqueIdGenerator.GetUniqueId(),
                PublicKey = keyPair.PublicKey,
                PrivateKey = symmetricProvider.EncryptWithKey(keyPair.PrivateKey, passphrase),
                PrivateKeyHash = SymmetricCryptoProvider.GetSecureHashForString(keyPair.PrivateKey),
                IsPrivateKeyEncrypted = true,
                IsPublicKeyOnly = false,
                Notifications = new EntityCollection<Notification>(),
                Messages = new EntityCollection<Message>(),
            };
        }

        public static CryptoKey CreateWithProvidedKeys(string publicKey, string privateKey, bool isPrivateKeyEncrypted, string privateKeyHash = null)
        {
            if (string.IsNullOrWhiteSpace(publicKey))
                throw new ArgumentException("Public key cannot be empty.", nameof(publicKey));

            if (string.IsNullOrWhiteSpace(privateKey))
                throw new ArgumentException("Private key cannot be empty.", nameof(privateKey));

            return new CryptoKey
            {
                RequestDate = DateTime.UtcNow,
                ReleaseDate = DateTime.UtcNow,
                KeyToken = UniqueIdGenerator.GetUniqueId(),
                PublicKey = publicKey,
                PrivateKey = privateKey,
                PrivateKeyHash = privateKeyHash,
                IsPrivateKeyEncrypted = isPrivateKeyEncrypted,
                Notifications = new EntityCollection<Notification>(),
                Messages = new EntityCollection<Message>(),
            };
        }

        #region Derived Properties

        [NotMapped]
        public bool IsReleased { get { return ReleaseDate <= DateTime.UtcNow; } }

        [XmlElement("PrivateKey")]
        [NotMapped]
        public string GetPrivateKey
        {
            get
            {
                return IsReleased ? this.PrivateKey : null;
            }
        }

        #endregion Derived Properties

        [Key]
        public int KeyId { get; set; }

        [Display(Name = "Key Id")]
        public string KeyToken { get; set; }

        [Display(Name = "Public Key")]
        public string PublicKey { get; set; }

        [XmlIgnore()]
        public string PrivateKey { get; set; }

        [XmlIgnore()]
        public string PrivateKeyHash { get; set; }

        public bool IsPrivateKeyEncrypted { get; set; }

        public bool IsPublicKeyOnly { get; set; }

        #region Extended Properties

        public DateTime RequestDate { get; set; }

        public DateTime ReleaseDate { get; set; }

        public DateTime? LockDate { get; set; }

        public bool DeleteMessagesAfterReading { get; set; }

        public bool DeleteKeyAfterReading { get; set; }

        #endregion Extended Properties

        [XmlIgnore()]
        public ICollection<Notification> Notifications { get; set; }

        [XmlIgnore()]
        public ICollection<Message> Messages { get; set; }

        [XmlElement("Messages")]
        public List<Message> MessagesSerialized
        {
            get
            {
                return Messages != null ? Messages.ToList() : new List<Message>();
            }
        }

        public string URI
        {
            get { return KeyToken.ToString(); }
        }


    }

    public class Notification
    {
        [Key]
        public int RequestId { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Enter a valid email address")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        public bool WasNotified { get; set; }
    }

    public class Message
    {
        public Message()
        {
            Created = DateTime.Now;
        }

        [Key]
        public int MessageId { get; set; }

        public string EncryptionKey { get; set; }

        [Required(ErrorMessage = "Please enter message data")]
        public string MessageData { get; set; }

        public DateTime Created { get; set; }

        public string MessageHash { get; set; }

        public bool IsFile { get; set; }

        [XmlIgnore()]
        [NotMapped]
        public string TemporaryDownloadId { get; set; }
    }
}
