using System;
using System.Security.Cryptography;
using System.Text;
using CryptAByte.CryptoLibrary.EncryptionLibraries;

namespace CryptAByte.CryptoLibrary.CryptoProviders
{
    public class SymmetricCryptoProvider : ICryptoProvider
    {
        public SymmetricCryptoProvider()
        {
            //    Salt = Guid.NewGuid().ToString().Substring(0, 8);
            Salt = "CryptAByte";
            //Salt = GenerateKeyPhrase(8);
        }

        public string Salt { get; set; }

        #region ICryptoProvider Members

        public string EncryptWithKey(string input, string key)
        {
            string encrypted = AESEncryption.Encrypt<AesManaged>(input, key, Salt);

            return encrypted;
        }

        public string DecryptWithKey(string encrypted, string key)
        {
            string decrypted = AESEncryption.Decrypt<AesManaged>(encrypted, key, Salt);
            return decrypted;
        }

        #endregion

        public static string GetSecureHashForString(string text)
        {
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] message = UE.GetBytes(text);

            SHA256Managed hashString = new SHA256Managed();
            string hex = "";

            byte[] hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        public static string GenerateKeyPhrase(int size = 128)
        {
            var rnd = RandomNumberGenerator.Create();
            byte[] randomNumber = new byte[size];
            rnd.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}