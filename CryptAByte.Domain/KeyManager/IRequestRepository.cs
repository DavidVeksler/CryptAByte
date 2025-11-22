using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptAByte.Domain.KeyManager
{
    public interface IRequestRepository
    {
        void AddRequest(CryptoKey request);
        CryptoKey GetRequest(string token);

        Task AttachMessageToRequestAsync(string token, string plainTextMessage);
        Task AttachEncryptedMessageToRequestAsync(string token, string encryptedMessage, string encryptionKey);
        Task AttachFileToRequestAsync(string keyToken, byte[] fileData, string fileName);

        List<Message> GetDecryptedMessagesWithPrivateKey(string token, string privateKey);
        List<Message> GetDecryptedMessagesWithPassphrase(string token, string passphrase);
        List<Message> GetEncryptedMessages(string token, string privateKeyHash);

        Message GetMessageByMessageId(int messageId);
        void DeleteKeyWithPassphrase(string token, string passphrase);
    }
}
