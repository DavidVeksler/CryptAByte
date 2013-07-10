using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using CryptAByte.WindowsClient.Commands;

namespace CryptAByte.WindowsClient
{
    public partial class Main : Form
    {
        private readonly ServiceProxy serviceProxy;

        public Main()
        {
            InitializeComponent();

            serviceProxy = new ServiceProxy();

            serviceProxy.ServiceUrl = "https://cryptabyte.com/Service/";
           // serviceProxy.ServiceUrl = "http://localhost:60888/Service/";

            Text = string.Format("Crypt-A-Byte Windows Client - (Alpha v {0}, API={1})", Environment.Version, serviceProxy.ServiceUrl);

            tabPage3.DragDrop += DropFileOnSendSelfDestructingEmail;

        }


        #region Create Key

        private void btnCreateKey_Click(object sender, EventArgs e)
        {
            if (txtPassphrase.Text.Length < 1)
            {
                MessageBox.Show("A passphrase is required to create a key.", "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                return;
            }

            string passphrase = txtPassphrase.Text;

            var key = serviceProxy.CreateKey(passphrase);

            if (key == null) return;

            txtKeyId.Text = key.KeyToken;
        }

        #endregion Create Key

        #region Send Message

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (txtKeyId.Text.Length < 16)
            {
                MessageBox.Show("A Key Id is required to send messages.", "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                return;
            }

            if (txtMessage.Text.Length < 1)
            {
                MessageBox.Show("Please enter a message.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            prgSendProgress.Value = 1;

            Cursor = Cursors.WaitCursor;

            bool success = serviceProxy.SendMessage(txtKeyId.Text, txtMessage.Text);

            Cursor = Cursors.Default;

            prgSendProgress.Value = 100;

            if (success)
            {
                txtMessage.Text = string.Empty;

                MessageBox.Show(Messages.MessageSent, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Unknown error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAttachFiles_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not yet implemented.");
        }

        private void lnkSharingUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (txtKeyId.Text.Length < 16)
            {
                MessageBox.Show("A Key Id is required");
                return;
            }

            string url = string.Format("https://cryptabyte.com/#{0}", txtKeyId.Text);
            Process.Start(url);
        }

        #endregion Send Message

        #region Get Messages

        private void btnGetMessages_Click(object sender, EventArgs e)
        {
            dvMessages.DataSource = null;

            var messages = serviceProxy.GetMessages(txtRetrieveKeyId.Text, txtRetrievePassprase.Text);

            if (messages == null) return;

            if (messages.Count == 0)
            {
                txtGetMessageStatus.Text += "0 messages retrieved. ";

                MessageBox.Show("No messages recieved.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            dvMessages.DataSource = messages.ToList().Select(m => new { Message = m.MessageData, Sent = m.Created }).ToList();

            txtGetMessageStatus.Text += messages.Count + " messages retrieved. ";
        }

        #endregion Get Messages

        #region Self-Destructing Messaging

        private string _selfDestructingFilePath;

        private string selfDestructingFilePath
        {
            get { return _selfDestructingFilePath; }
            set
            {
                _selfDestructingFilePath = value;
                lblSelfDestructSelectedFile.Text = value;
            }
        }

        private void btnAttachFile_Click(object sender, EventArgs e)
        {
            DialogResult result = this.openFileDialogToSendEmail.ShowDialog();

            if (result == DialogResult.OK)
            {
                selfDestructingFilePath = openFileDialogToSendEmail.FileName;
            }
            else
            {
                selfDestructingFilePath = string.Empty;
                lblSelfDestructSelectedFile.Text = Messages.NoFilesSelected;
            }
        }

        private void DropFileOnSendSelfDestructingEmail(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filePath in files)
            {
                selfDestructingFilePath = filePath;
            }
        }

        private void btnSendSelfDestructingMessage_Click(object sender, EventArgs e)
        {
            #region Validation

            if (!Domain.Validation.IsValidEmail(txtSelfDestructingEmail.Text))
            {
                MessageBox.Show("Email required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtSelfDestructingMessage.Text.Length < 1)
            {
                MessageBox.Show("Please enter a message.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            #endregion Validation

            btnSendSelfDestructingMessage.Text = "Sending...";

            Cursor = Cursors.WaitCursor;
            bool success = serviceProxy.SendSelfDestructingMessage(txtSelfDestructingEmail.Text, txtSelfDestructingMessage.Text,selfDestructingFilePath);
            Cursor = Cursors.Default;

            if (success)
            {
                txtMessage.Text = string.Empty;

                MessageBox.Show(Messages.MessageSent, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // reset state
                ResetSelfDestructFormToDefault();
            }
            else
            {
                MessageBox.Show("Message not sent.  Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
            

            btnSendSelfDestructingMessage.Text = "Send Message";

            

        }

        private void ResetSelfDestructFormToDefault()
        {
            selfDestructingFilePath = string.Empty;
            txtSelfDestructingMessage.Text = string.Empty;
            txtSelfDestructingEmail.Text = string.Empty;
            lblSelfDestructSelectedFile.Text = Messages.NoFilesSelected;
        }

        #endregion Self-Destructing Messaging


    }
}