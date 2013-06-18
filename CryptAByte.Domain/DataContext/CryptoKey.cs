using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CryptAByte.CryptoLibrary.CryptoProviders;
using CryptAByte.Domain.DataContext;

namespace CryptAByte.Domain.KeyManager
{
    public sealed class CryptoKey
    {
        #region Constructors

        /// <summary>
        /// only use for Serialization
        /// </summary>
        public CryptoKey() { }

        public static CryptoKey CreateRequest(DateTime? releaseDate = null)
        {
            var key = AsymmetricCryptoProvider.GenerateKeys();

            var request = new CryptoKey
                              {
                                  RequestDate = DateTime.Now,
                                  ReleaseDate = releaseDate ?? DateTime.Now,
                                  KeyToken = UniqueIdGenerator.GetUniqueId(),
                                  PrivateKey = key.PrivateKey,
                                  PublicKey = key.PublicKey,
                                  IsPrivateKeyEncrypted = false,
                                  IsPublicKeyOnly = false,
                                  Notifications = new EntityCollection<Notification>(),
                                  Messages = new EntityCollection<Message>(),
                              };


            return request;
        }

        public static CryptoKey CreateRequestWithPassPhrase(string passphrase)
        {
            var key = AsymmetricCryptoProvider.GenerateKeys();

            var request = new CryptoKey
                              {
                                  RequestDate = DateTime.UtcNow,
                                  ReleaseDate = DateTime.Now,
                                  KeyToken = UniqueIdGenerator.GetUniqueId(),
                                  PublicKey = key.PublicKey,
                                  PrivateKey = new SymmetricCryptoProvider().EncryptWithKey(key.PrivateKey, passphrase),
                                  PrivateKeyHash = SymmetricCryptoProvider.GetSecureHashForString(key.PrivateKey),
                                  IsPrivateKeyEncrypted = true,
                                  IsPublicKeyOnly = false,
                                  Notifications = new EntityCollection<Notification>(),
                                  Messages = new EntityCollection<Message>(),
                              };

            return request;
        }

        public static CryptoKey CreateRequestWithPublicKey(string publicKey, string privatekey, bool IsPrivateKeyEncrypted, string privateKeyHash = null)
        {
            var request = new CryptoKey
                              {
                                  RequestDate = DateTime.UtcNow,
                                  ReleaseDate = DateTime.Now,
                                  KeyToken = UniqueIdGenerator.GetUniqueId(),
                                  PublicKey = publicKey,
                                  PrivateKey = privatekey,
                                  PrivateKeyHash = privateKeyHash,
                                  IsPrivateKeyEncrypted = IsPrivateKeyEncrypted,
                                  Notifications = new EntityCollection<Notification>(),
                                  Messages = new EntityCollection<Message>(),
                              };

            return request;
        }



        public CryptoKey(DateTime releaseDate, DateTime requestDate, string token, KeyPair keys)
        {
            ReleaseDate = releaseDate;
            RequestDate = requestDate;
            KeyToken = token;
            PrivateKey = keys.PrivateKey;
            PublicKey = keys.PublicKey;
        }

        #endregion Constructors

        #region Derived Properties

        [NotMapped]
        public bool IsReleased { get { return ReleaseDate < DateTime.Now; } }

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

        //[EditorVisibile(EditorVisibility.Advanced)]
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

        [System.ComponentModel.DataAnnotations.Timestamp]
        //      [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; set; }

        public string MessageHash { get; set; }

        public bool IsFile { get; set; }

        [XmlIgnore()]
        [NotMapped]
        public string TemporaryDownloadId { get; set; }
    }

    public class CryptAByteContext : DbContext
    {
        public DbSet<CryptoKey> Keys { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<SelfDestructingMessage> SelfDestructingMessages { get; set; }

        public DbSet<SelfDestructingMessageAttachment> SelfDestructingMessageAttachments { get; set; }
    }

    
}
