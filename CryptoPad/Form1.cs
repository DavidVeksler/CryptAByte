using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using CryptAByte.CryptoLibrary.CryptoProviders;

namespace CryptoPad
{
    //ILMerge.exe /target:CryptoPad /out:CryptoPad2.exe /targetplatform:"v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" CryptoPad.exe CryptAByte.CryptoLibrary.dll

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            SymmetricCryptoProvider symmetricCrypt = new SymmetricCryptoProvider();

            string password = txtPassphrase.Text;

            if (string.IsNullOrWhiteSpace(password))
            {
                password = PronounceablePasswordGenerator.Generate(10);
                MessageBox.Show(
                    string.Format("Your random password is {0}.  \r Please copy it now then click OK.", password),
                    "Password generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            string cypertext = symmetricCrypt.EncryptWithKey(txtClearText.Text, password);

            txtCypertext.Text = cypertext;

            tabControl1.SelectTab(1);
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            SymmetricCryptoProvider symmetricCrypt = new SymmetricCryptoProvider();

            if (string.IsNullOrWhiteSpace(txtDecryptPassphrase.Text))
            {
                MessageBox.Show("Cannot decrypt without a passphrase", "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            string plaintext = "";

            try
            {
                plaintext = symmetricCrypt.DecryptWithKey(txtCypertext.Text, txtDecryptPassphrase.Text);
            }
            catch (CryptographicException)
            {
                MessageBox.Show("Unable to decrypt with provided passphrase", "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            txtCypertext.Text = plaintext;

            tabControl1.SelectTab(0);
        }
    }
}