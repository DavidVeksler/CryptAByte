using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptAByte.CryptoLibrary.EncryptionLibraries;

namespace CryptAByte.CryptoLibrary.CryptoProviders
{
    public class SymmetricCryptoProvider : ICryptoProvider
    {
        private const string DefaultEncryptionSalt = "CryptAByte";
        private readonly string _encryptionSalt;

        public SymmetricCryptoProvider() : this(DefaultEncryptionSalt)
        {
        }

        public SymmetricCryptoProvider(string encryptionSalt)
        {
            _encryptionSalt = encryptionSalt ?? throw new ArgumentNullException(nameof(encryptionSalt));
        }

        public string EncryptWithKey(string plaintext, string encryptionKey)
        {
            return AESEncryption.Encrypt<AesManaged>(plaintext, encryptionKey, _encryptionSalt);
        }

        public string DecryptWithKey(string ciphertext, string decryptionKey)
        {
            return AESEncryption.Decrypt<AesManaged>(ciphertext, decryptionKey, _encryptionSalt);
        }

        public static string GetSecureHashForString(string text)
        {
            byte[] messageBytes = Encoding.Unicode.GetBytes(text);

            using (var hashAlgorithm = SHA256.Create())
            {
                byte[] hashValue = hashAlgorithm.ComputeHash(messageBytes);
                return string.Concat(hashValue.Select(b => b.ToString("x2")));
            }
        }

        public static string GenerateKeyPhrase(int sizeInBytes = 128)
        {
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[sizeInBytes];
                randomNumberGenerator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }
}