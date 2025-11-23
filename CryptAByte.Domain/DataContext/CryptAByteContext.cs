using System.Data.Entity;
using CryptAByte.Domain.KeyManager;

namespace CryptAByte.Domain.DataContext
{
    public class CryptAByteContext : DbContext
    {
        public DbSet<CryptoKey> Keys { get; set; }
        // public DbSet<Notification> Notifications { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<SelfDestructingMessage> SelfDestructingMessages { get; set; }
        public DbSet<SelfDestructingMessageAttachment> SelfDestructingMessageAttachments { get; set; }
    }
}
