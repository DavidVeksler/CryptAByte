namespace CryptAByte.WindowsClient
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.btnCreateKey = new System.Windows.Forms.Button();
            this.txtPassphrase = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRetrievePassprase = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnAttachFiles = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lnkSharingUrl = new System.Windows.Forms.LinkLabel();
            this.txtKeyId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.prgSendProgress = new System.Windows.Forms.ProgressBar();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtGetMessageStatus = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtRetrieveKeyId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dvMessages = new System.Windows.Forms.DataGridView();
            this.btnGetMessages = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lblSelfDestructSelectedFile = new System.Windows.Forms.Label();
            this.btnAttachFile = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSelfDestructingEmail = new System.Windows.Forms.TextBox();
            this.btnSendSelfDestructingMessage = new System.Windows.Forms.Button();
            this.txtSelfDestructingMessage = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openFileDialogToSendEmail = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dvMessages)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCreateKey
            // 
            this.btnCreateKey.Location = new System.Drawing.Point(280, 10);
            this.btnCreateKey.Name = "btnCreateKey";
            this.btnCreateKey.Size = new System.Drawing.Size(120, 23);
            this.btnCreateKey.TabIndex = 0;
            this.btnCreateKey.Text = "&Create Key Id";
            this.btnCreateKey.UseVisualStyleBackColor = true;
            this.btnCreateKey.Click += new System.EventHandler(this.btnCreateKey_Click);
            // 
            // txtPassphrase
            // 
            this.txtPassphrase.Location = new System.Drawing.Point(85, 13);
            this.txtPassphrase.Name = "txtPassphrase";
            this.txtPassphrase.Size = new System.Drawing.Size(189, 20);
            this.txtPassphrase.TabIndex = 1;
            this.txtPassphrase.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Passphrase";
            // 
            // txtRetrievePassprase
            // 
            this.txtRetrievePassprase.Location = new System.Drawing.Point(98, 45);
            this.txtRetrievePassprase.MaxLength = 64;
            this.txtRetrievePassprase.Name = "txtRetrievePassprase";
            this.txtRetrievePassprase.Size = new System.Drawing.Size(192, 20);
            this.txtRetrievePassprase.TabIndex = 7;
            this.txtRetrievePassprase.UseSystemPasswordChar = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(0, 65);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(492, 505);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(484, 479);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Send Messages";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.btnAttachFiles);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lnkSharingUrl);
            this.groupBox2.Controls.Add(this.txtKeyId);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnSendMessage);
            this.groupBox2.Controls.Add(this.prgSendProgress);
            this.groupBox2.Controls.Add(this.txtMessage);
            this.groupBox2.Location = new System.Drawing.Point(8, 48);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(459, 428);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Send Messages";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(158, 346);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "(No files selected)";
            // 
            // btnAttachFiles
            // 
            this.btnAttachFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAttachFiles.Location = new System.Drawing.Point(9, 346);
            this.btnAttachFiles.Name = "btnAttachFiles";
            this.btnAttachFiles.Size = new System.Drawing.Size(126, 23);
            this.btnAttachFiles.TabIndex = 19;
            this.btnAttachFiles.Text = "Attach Files (TODO)";
            this.btnAttachFiles.UseVisualStyleBackColor = true;
            this.btnAttachFiles.Click += new System.EventHandler(this.btnAttachFiles_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Key Id:";
            // 
            // lnkSharingUrl
            // 
            this.lnkSharingUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkSharingUrl.AutoSize = true;
            this.lnkSharingUrl.Location = new System.Drawing.Point(289, 23);
            this.lnkSharingUrl.Name = "lnkSharingUrl";
            this.lnkSharingUrl.Size = new System.Drawing.Size(88, 13);
            this.lnkSharingUrl.TabIndex = 12;
            this.lnkSharingUrl.TabStop = true;
            this.lnkSharingUrl.Text = "Open Sharing Url";
            this.lnkSharingUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSharingUrl_LinkClicked);
            // 
            // txtKeyId
            // 
            this.txtKeyId.Location = new System.Drawing.Point(85, 23);
            this.txtKeyId.Name = "txtKeyId";
            this.txtKeyId.Size = new System.Drawing.Size(189, 20);
            this.txtKeyId.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Message:";
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSendMessage.Location = new System.Drawing.Point(340, 383);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(94, 24);
            this.btnSendMessage.TabIndex = 15;
            this.btnSendMessage.Text = "&Send";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // prgSendProgress
            // 
            this.prgSendProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.prgSendProgress.Location = new System.Drawing.Point(9, 383);
            this.prgSendProgress.Name = "prgSendProgress";
            this.prgSendProgress.Size = new System.Drawing.Size(210, 24);
            this.prgSendProgress.TabIndex = 13;
            // 
            // txtMessage
            // 
            this.txtMessage.AcceptsReturn = true;
            this.txtMessage.AcceptsTab = true;
            this.txtMessage.AllowDrop = true;
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.Location = new System.Drawing.Point(6, 86);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(428, 254);
            this.txtMessage.TabIndex = 14;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnCreateKey);
            this.groupBox1.Controls.Add(this.txtPassphrase);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(459, 39);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Create New Key Id";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtGetMessageStatus);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.txtRetrieveKeyId);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.dvMessages);
            this.tabPage2.Controls.Add(this.btnGetMessages);
            this.tabPage2.Controls.Add(this.txtRetrievePassprase);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(484, 479);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "View Messages";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtGetMessageStatus
            // 
            this.txtGetMessageStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGetMessageStatus.Location = new System.Drawing.Point(23, 300);
            this.txtGetMessageStatus.Multiline = true;
            this.txtGetMessageStatus.Name = "txtGetMessageStatus";
            this.txtGetMessageStatus.ReadOnly = true;
            this.txtGetMessageStatus.Size = new System.Drawing.Size(444, 160);
            this.txtGetMessageStatus.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Key Id:";
            // 
            // txtRetrieveKeyId
            // 
            this.txtRetrieveKeyId.Location = new System.Drawing.Point(98, 11);
            this.txtRetrieveKeyId.Name = "txtRetrieveKeyId";
            this.txtRetrieveKeyId.Size = new System.Drawing.Size(189, 20);
            this.txtRetrieveKeyId.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Passphrase";
            // 
            // dvMessages
            // 
            this.dvMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dvMessages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvMessages.Location = new System.Drawing.Point(23, 87);
            this.dvMessages.Name = "dvMessages";
            this.dvMessages.Size = new System.Drawing.Size(443, 196);
            this.dvMessages.TabIndex = 0;
            // 
            // btnGetMessages
            // 
            this.btnGetMessages.Location = new System.Drawing.Point(296, 43);
            this.btnGetMessages.Name = "btnGetMessages";
            this.btnGetMessages.Size = new System.Drawing.Size(171, 23);
            this.btnGetMessages.TabIndex = 10;
            this.btnGetMessages.Text = "&Get Messages";
            this.btnGetMessages.UseVisualStyleBackColor = true;
            this.btnGetMessages.Click += new System.EventHandler(this.btnGetMessages_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.AllowDrop = true;
            this.tabPage3.Controls.Add(this.lblSelfDestructSelectedFile);
            this.tabPage3.Controls.Add(this.btnAttachFile);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.txtSelfDestructingEmail);
            this.tabPage3.Controls.Add(this.btnSendSelfDestructingMessage);
            this.tabPage3.Controls.Add(this.txtSelfDestructingMessage);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(484, 479);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Self-Destructing Email";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // lblSelfDestructSelectedFile
            // 
            this.lblSelfDestructSelectedFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSelfDestructSelectedFile.Location = new System.Drawing.Point(94, 427);
            this.lblSelfDestructSelectedFile.Name = "lblSelfDestructSelectedFile";
            this.lblSelfDestructSelectedFile.Size = new System.Drawing.Size(240, 43);
            this.lblSelfDestructSelectedFile.TabIndex = 25;
            this.lblSelfDestructSelectedFile.Text = "No files selected. \r\nClick or drag to select.";
            // 
            // btnAttachFile
            // 
            this.btnAttachFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAttachFile.Location = new System.Drawing.Point(18, 427);
            this.btnAttachFile.Name = "btnAttachFile";
            this.btnAttachFile.Size = new System.Drawing.Size(66, 43);
            this.btnAttachFile.TabIndex = 24;
            this.btnAttachFile.Text = "Attach \r\n&File";
            this.btnAttachFile.UseVisualStyleBackColor = true;
            this.btnAttachFile.Click += new System.EventHandler(this.btnAttachFile_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 23;
            this.label8.Text = "Message:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Email:";
            // 
            // txtSelfDestructingEmail
            // 
            this.txtSelfDestructingEmail.Location = new System.Drawing.Point(97, 20);
            this.txtSelfDestructingEmail.Name = "txtSelfDestructingEmail";
            this.txtSelfDestructingEmail.Size = new System.Drawing.Size(189, 20);
            this.txtSelfDestructingEmail.TabIndex = 21;
            // 
            // btnSendSelfDestructingMessage
            // 
            this.btnSendSelfDestructingMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSendSelfDestructingMessage.Location = new System.Drawing.Point(352, 427);
            this.btnSendSelfDestructingMessage.Name = "btnSendSelfDestructingMessage";
            this.btnSendSelfDestructingMessage.Size = new System.Drawing.Size(94, 43);
            this.btnSendSelfDestructingMessage.TabIndex = 20;
            this.btnSendSelfDestructingMessage.Text = "Send &Message";
            this.btnSendSelfDestructingMessage.UseVisualStyleBackColor = true;
            this.btnSendSelfDestructingMessage.Click += new System.EventHandler(this.btnSendSelfDestructingMessage_Click);
            // 
            // txtSelfDestructingMessage
            // 
            this.txtSelfDestructingMessage.AcceptsReturn = true;
            this.txtSelfDestructingMessage.AcceptsTab = true;
            this.txtSelfDestructingMessage.AllowDrop = true;
            this.txtSelfDestructingMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSelfDestructingMessage.Location = new System.Drawing.Point(18, 88);
            this.txtSelfDestructingMessage.Multiline = true;
            this.txtSelfDestructingMessage.Name = "txtSelfDestructingMessage";
            this.txtSelfDestructingMessage.Size = new System.Drawing.Size(428, 323);
            this.txtSelfDestructingMessage.TabIndex = 19;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Image = global::CryptAByte.WindowsClient.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(492, 64);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 570);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 600);
            this.Name = "Main";
            this.Text = "Crypt-A-Byte Windows Client";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dvMessages)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCreateKey;
        private System.Windows.Forms.TextBox txtPassphrase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRetrievePassprase;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dvMessages;
        private System.Windows.Forms.Button btnGetMessages;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnAttachFiles;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtKeyId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.ProgressBar prgSendProgress;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel lnkSharingUrl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtGetMessageStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtRetrieveKeyId;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSelfDestructingEmail;
        private System.Windows.Forms.Button btnSendSelfDestructingMessage;
        private System.Windows.Forms.TextBox txtSelfDestructingMessage;
        private System.Windows.Forms.Label lblSelfDestructSelectedFile;
        private System.Windows.Forms.Button btnAttachFile;
        private System.Windows.Forms.OpenFileDialog openFileDialogToSendEmail;
    }
}

