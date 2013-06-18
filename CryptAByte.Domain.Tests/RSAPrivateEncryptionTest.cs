using CryptAByte.CryptoLibrary.EncryptionLibraries;
using RSAExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Numerics;
using System.Text;

namespace RSAExtensions.Tests
{
    [TestClass]
    public class RSAPrivateEncryptionTest
    {

        /// <summary>
        ///A test for PrivateEncryption
        ///</summary>
        [TestMethod]
        public void PrivateEncryptionTest()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            Random rnd = new Random();
            byte[] data;

            for (int i = 0; i < 200; i++)
            {
                data = new byte[rnd.Next(16, 123)];
                rnd.NextBytes(data);  // Random content

                // Private encrypt this data
                byte[] cipher = rsa.PrivateEncryption(data);

                // And decrypt it back
                byte[] decipher = rsa.PublicDecryption(cipher);
                Assert.AreEqual(data.Length, decipher.Length, "Round: {0}, data: {1}, decipher: {2}",
                    i, ShowBytesValue(data), ShowBytesValue(decipher));
                Assert.IsTrue(data.SequenceEqual(decipher), "Round: {0} data: {1} decipher: {2}",
                    i, ShowBytesValue(data), ShowBytesValue(decipher));
            }
        }

        [TestMethod]
        public void PrivateEncryption_Load_Key_Test()
        {
            Random rnd = new Random();
            byte[] data = new byte[196];
            rnd.NextBytes(data);
            byte[] encData;

            using (RSACryptoServiceProvider privateRsa = new RSACryptoServiceProvider())
            {
                string privateKey =
                    "<RSAKeyValue><Modulus>vKzm/rFi2jPG6fumvhDWeZ49ZZhUh5EEXWW5fs1Y5iFW2poGA9I2sdeBBVgE16DshQY+VdW+e4uaXZJncuy+MdHoe9HWGx6iAVB3PSFUGNhaMQnX+bv+GWORBoFMBpx2ZGUqOSbazIxBpTxyg0DnXdpeFbVrWONq/A7RsmLWHZKk1cdoJ15YPI85IPFO3YwsfBDCYMHaaVcy7Ac9UvJGasopzfQq7dPM8d0xj93VUPq8La4psejJ/N56IYjs/+rvoFvACbt8U+a9UMWjNVbnBm4+MjcxASbxLkd3izRmZ5Jtv8YRoHOKkxlgG8vVVcNPNONoMNdE+YmxeZh7pH1w0Q==</Modulus><Exponent>AQAB</Exponent><P>3Wj62DgD6UFsoOxlgr7FpDM7Ev3/K4IfdHUkznGpVXWALawFMNMLmMIvp1NwFJ9U05+6IA3xnoq0Ue7mrJZ4HPqb7qwrJ9hG5gpXZzyMVUvcblJxchzet6N5fb3yMDX3A6zwfrsnaYucHVWmdf/MhWtkM/jQQN+Z1kAxkB+NTz0=</P><Q>2ia9ZGUKmw8Jsn0Ew13OkYzyjyw6E5ffwEDafpwJ8AojoKnFI1b4NSkYt3oAQbamLTyupoxL+3DQ7d7TeMoQqunM/tWOukZo3rAyaa/KRoNIcbDNLTdtzpdxgfZmZh6WrEHNkDx5y/UGqb/ceVlKeDCSQvE+sTuagbxCPoLkwSU=</Q><DP>xjGYAf66eY1oGPEjuRLeRqrZYZneVesIDy5hgS87flVNJRUMHHV+twJ0t9q3xK4Pt9QOP21b8SiGW6V39dxHruEivlZ91xAB/yAYtz/6+suKiXLhPF3dfBMoyMdESaW09SRUr40GrbMcTyIBfTU6td+49dDvUnMV+TTDaRjlXJ0=</DP><DQ>DJDUsfa8AKiCF3zqDFLX9jxXMHYMtlo2Mj3KGCbmz6PV34hH6bw1ueIvIUpuv1pFAjAPo1pLeiVKc5k1Nyz0ftPO0hL9EK/DlKgzjzDoBt3DC4FyoBskQRUqHaFSzqkOZse3jopdPalUg+ygR4EkL/4kPqTkxpK3WKe+bRlfEd0=</DQ><InverseQ>gSF7iEokHbW9m4IgZa3HL0fDcLAQUK6JO4uq/RM5/VgHtNR785OLU3pQjqHGgu2SmtwitNf+R8TKHkz8/777qKk3MUb9SRAApKFomSPQXU8Iu3m/WJuYCfFq+MeNCyNQ+UNsD7qqI6Xie/wsExMOa8+HP7VsxhOX7btVqErz5+0=</InverseQ><D>nW70zJ79ai98Ei/O4ZexLwgQGR7zoa8q4jgIgTsdq+Ez1PJihHu68chtuyTH3ZlE4nbkOsFA0VwasWuBcI8E4RNTF0Zvjm+QJOKcrGCMCLM3BuY81gC8tTi0gaYP5xBVZc5YXhoCxl1eRV9b+hOFO3YDvb+E1EXnNm2zIlOAcGlG44ezABU+MypPPEWr00DVG9AYSAjS93DoCPO01EMPJCf8eRcIQpnjJf22o0+0fgI3rkfkuJI6W1W16sqHHYJMv3KNo7kakL7OzKUcAZgCKx7IPQ9ilsYmKe0Ffj4qsEPrk2lIts/S9rYJWAoqBh1PdPh5F2P+adDz/A1HY4ZUsQ==</D></RSAKeyValue>";

                privateRsa.FromXmlString(privateKey);
                encData = privateRsa.PrivateEncryption(data);
            }

            byte[] decipher;
            using (RSACryptoServiceProvider publicRsa = new RSACryptoServiceProvider())
            {
                string publicKey =
                    "<RSAKeyValue><Modulus>vKzm/rFi2jPG6fumvhDWeZ49ZZhUh5EEXWW5fs1Y5iFW2poGA9I2sdeBBVgE16DshQY+VdW+e4uaXZJncuy+MdHoe9HWGx6iAVB3PSFUGNhaMQnX+bv+GWORBoFMBpx2ZGUqOSbazIxBpTxyg0DnXdpeFbVrWONq/A7RsmLWHZKk1cdoJ15YPI85IPFO3YwsfBDCYMHaaVcy7Ac9UvJGasopzfQq7dPM8d0xj93VUPq8La4psejJ/N56IYjs/+rvoFvACbt8U+a9UMWjNVbnBm4+MjcxASbxLkd3izRmZ5Jtv8YRoHOKkxlgG8vVVcNPNONoMNdE+YmxeZh7pH1w0Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

                publicRsa.FromXmlString(publicKey);
                decipher = publicRsa.PublicDecryption(encData);
            }

            Assert.IsTrue(decipher.SequenceEqual(data));
        }

        [TestMethod]
        public void RSACryptoServiceProvider_Usage()
        {
            string secret = "My secret message";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(512);  // Key bits length
            /*
             * Skip the loading part for the RSACryptoServiceProvider will generate
             * random Private / Public keys pair, that you can save later with
             * rsa.ToXmlString(true);
             * 
            string key = "private or public key as xml string";
            rsa.FromXmlString(key);
             */
            // Convert the string to byte array
            byte[] secretData = Encoding.UTF8.GetBytes(secret);

            // Encrypt it using the private key:
            byte[] encrypted = rsa.PrivateEncryption(secretData);

            // Decrypt it using the public key
            byte[] decrypted = rsa.PublicDecryption(encrypted);
            string decString = Encoding.UTF8.GetString(decrypted);  // And back to string
            Assert.AreEqual("My secret message", decString);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PrivateEncryption_Throws_Exception_On_Large_Data_Length()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            Random rnd = new Random();
            byte[] data = new byte[123];
            rnd.NextBytes(data);
            byte[] encData = rsa.PrivateEncryption(data);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PrivateEncryption_Throws_Exception_On_PublicKey_Only()
        {
            // Public key:
            string publicKey = "<RSAKeyValue><Modulus>si1hKcwL9ACnfOHpZooo2hHxvTMjFnDTaeNW6RT03FanHKK56t6qGAcWwwlLGi/gZZmJGqJQ/7yfPkrKmyg4zvHrND8cPiue3GylKFr+/NeDY4Hx/LP6r8nAJ+HxBTiFNwlZoH/Ut0NlYWVdbtFjXWCh9pqSpCz1pEyeLaUXo/k=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            Random rnd = new Random();
            byte[] data = new byte[64];
            rnd.NextBytes(data);
            byte[] encData = rsa.PrivateEncryption(data);
        }

        [TestMethod]
        //[Ignore]
        public void GenerateKey()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            Console.WriteLine(rsa.ToXmlString(true));
            Console.WriteLine(rsa.ToXmlString(false));

        }

        private string ShowBytesValue(byte[] data)
        {
            StringBuilder builder = new StringBuilder().Append("{ ");
            for (int i = 0; i < data.Length -1; i++)
                builder.Append(data[i]).Append(", ");

            builder.Append(data[data.Length - 1] + " }");
            return builder.ToString();
        }

    }
}
