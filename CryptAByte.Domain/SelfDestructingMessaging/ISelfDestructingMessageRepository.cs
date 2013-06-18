using CryptAByte.Domain.DataContext;

namespace CryptAByte.Domain.SelfDestructingMessaging
{
    public interface ISelfDestructingMessageRepository
    {
       
        SelfDestructingMessage GetMessage(int messageId, string passphrase);
        SelfDestructingMessageAttachment GetAttachment(int messageId, string passphrase);
        int StoreMessage(SelfDestructingMessage selfDestructingMessage, string passphrase, string attachmentName = null, byte[] attachment = null);
    }
}