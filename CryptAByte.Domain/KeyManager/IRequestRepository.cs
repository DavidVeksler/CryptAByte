using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptAByte.Domain.KeyManager
{
    public interface IRequestRepository
    {
        void AddRequest(CryptoKey request);
        CryptoKey GetRequest(string token);
        void AttachMessageToRequest(string token, string plainTextMessage);
        void AttachFileToRequest(string keyToken, byte[] fileData, string fileName);

        List<Message> GetDecryptedMessagesWithPrivateKey(string token, string privateKey);
        List<Message> GetDecryptedMessagesWithPassphrase(string token, string passphrase);

        List<Message> GetEncryptedMessages(string token, string privateKeyHash);

        Message GetMessageByMessageId(int messageId);
        void DeleteKeyWithPassphrase(string token, string passphrase);
        void NotifyOnMessageReceived(string token);
        
        void AttachEncryptedMessageToRequest(string token, string encryptedmessage, string encryptionkey);
    }
}
