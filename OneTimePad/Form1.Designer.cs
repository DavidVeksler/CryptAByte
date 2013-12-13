namespace OneTimePad
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.txtkeys = new System.Windows.Forms.TextBox();
            this.btnGenerateKeys = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numKeySize = new System.Windows.Forms.NumericUpDown();
            this.numNumberOfKeys = new System.Windows.Forms.NumericUpDown();
            this.chkCreatepronounceablepasswords = new System.Windows.Forms.CheckBox();
            this.chkGroupIntoPairs = new System.Windows.Forms.CheckBox();
            this.numGroupSize = new System.Windows.Forms.NumericUpDown();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.numKeySize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumberOfKeys)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGroupSize)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 462);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Key size";
            // 
            // txtkeys
            // 
            this.txtkeys.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtkeys.Location = new System.Drawing.Point(0, 0);
            this.txtkeys.Multiline = true;
            this.txtkeys.Name = "txtkeys";
            this.txtkeys.Size = new System.Drawing.Size(605, 450);
            this.txtkeys.TabIndex = 5;
            // 
            // btnGenerateKeys
            // 
            this.btnGenerateKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerateKeys.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerateKeys.Location = new System.Drawing.Point(344, 462);
            this.btnGenerateKeys.Name = "btnGenerateKeys";
            this.btnGenerateKeys.Size = new System.Drawing.Size(240, 48);
            this.btnGenerateKeys.TabIndex = 4;
            this.btnGenerateKeys.Text = "Generate Keys";
            this.btnGenerateKeys.UseVisualStyleBackColor = true;
            this.btnGenerateKeys.Click += new System.EventHandler(this.btnGenerateKeys_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 490);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Number of Keys";
            // 
            // numKeySize
            // 
            this.numKeySize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numKeySize.Location = new System.Drawing.Point(100, 462);
            this.numKeySize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numKeySize.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numKeySize.Name = "numKeySize";
            this.numKeySize.Size = new System.Drawing.Size(120, 20);
            this.numKeySize.TabIndex = 9;
            this.numKeySize.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // numNumberOfKeys
            // 
            this.numNumberOfKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numNumberOfKeys.Location = new System.Drawing.Point(100, 488);
            this.numNumberOfKeys.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numNumberOfKeys.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumberOfKeys.Name = "numNumberOfKeys";
            this.numNumberOfKeys.Size = new System.Drawing.Size(120, 20);
            this.numNumberOfKeys.TabIndex = 10;
            this.numNumberOfKeys.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // chkCreatepronounceablepasswords
            // 
            this.chkCreatepronounceablepasswords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkCreatepronounceablepasswords.AutoSize = true;
            this.chkCreatepronounceablepasswords.Checked = true;
            this.chkCreatepronounceablepasswords.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCreatepronounceablepasswords.Location = new System.Drawing.Point(15, 514);
            this.chkCreatepronounceablepasswords.Name = "chkCreatepronounceablepasswords";
            this.chkCreatepronounceablepasswords.Size = new System.Drawing.Size(184, 17);
            this.chkCreatepronounceablepasswords.TabIndex = 11;
            this.chkCreatepronounceablepasswords.Text = "Create pronounceable passwords";
            this.chkCreatepronounceablepasswords.UseVisualStyleBackColor = true;
            // 
            // chkGroupIntoPairs
            // 
            this.chkGroupIntoPairs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkGroupIntoPairs.AutoSize = true;
            this.chkGroupIntoPairs.Location = new System.Drawing.Point(15, 534);
            this.chkGroupIntoPairs.Name = "chkGroupIntoPairs";
            this.chkGroupIntoPairs.Size = new System.Drawing.Size(115, 17);
            this.chkGroupIntoPairs.TabIndex = 12;
            this.chkGroupIntoPairs.Text = "Group into pairs of ";
            this.chkGroupIntoPairs.UseVisualStyleBackColor = true;
            // 
            // numGroupSize
            // 
            this.numGroupSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numGroupSize.Location = new System.Drawing.Point(136, 531);
            this.numGroupSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numGroupSize.Name = "numGroupSize";
            this.numGroupSize.Size = new System.Drawing.Size(56, 20);
            this.numGroupSize.TabIndex = 13;
            this.numGroupSize.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(303, 534);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(281, 13);
            this.linkLabel1.TabIndex = 14;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "You can use CryptoPad to encrypt and decrypt messages.";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 555);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.numGroupSize);
            this.Controls.Add(this.chkGroupIntoPairs);
            this.Controls.Add(this.chkCreatepronounceablepasswords);
            this.Controls.Add(this.numNumberOfKeys);
            this.Controls.Add(this.numKeySize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtkeys);
            this.Controls.Add(this.btnGenerateKeys);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "OneTimePad (Open Source Crypto from CryptAByte.com)";
            ((System.ComponentModel.ISupportInitialize)(this.numKeySize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumberOfKeys)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGroupSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtkeys;
        private System.Windows.Forms.Button btnGenerateKeys;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numKeySize;
        private System.Windows.Forms.NumericUpDown numNumberOfKeys;
        private System.Windows.Forms.CheckBox chkCreatepronounceablepasswords;
        private System.Windows.Forms.CheckBox chkGroupIntoPairs;
        private System.Windows.Forms.NumericUpDown numGroupSize;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}

