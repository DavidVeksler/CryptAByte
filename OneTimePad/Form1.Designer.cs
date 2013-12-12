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
            ((System.ComponentModel.ISupportInitialize)(this.numKeySize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumberOfKeys)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 471);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Key size";
            // 
            // txtkeys
            // 
            this.txtkeys.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtkeys.Location = new System.Drawing.Point(0, 0);
            this.txtkeys.Multiline = true;
            this.txtkeys.Name = "txtkeys";
            this.txtkeys.Size = new System.Drawing.Size(605, 461);
            this.txtkeys.TabIndex = 5;
            // 
            // btnGenerateKeys
            // 
            this.btnGenerateKeys.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerateKeys.Location = new System.Drawing.Point(424, 478);
            this.btnGenerateKeys.Name = "btnGenerateKeys";
            this.btnGenerateKeys.Size = new System.Drawing.Size(159, 48);
            this.btnGenerateKeys.TabIndex = 4;
            this.btnGenerateKeys.Text = "Generate";
            this.btnGenerateKeys.UseVisualStyleBackColor = true;
            this.btnGenerateKeys.Click += new System.EventHandler(this.btnGenerateKeys_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 499);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Number of Keys";
            // 
            // numKeySize
            // 
            this.numKeySize.Location = new System.Drawing.Point(100, 471);
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
            this.numNumberOfKeys.Location = new System.Drawing.Point(100, 497);
            this.numNumberOfKeys.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumberOfKeys.Name = "numNumberOfKeys";
            this.numNumberOfKeys.Size = new System.Drawing.Size(120, 20);
            this.numNumberOfKeys.TabIndex = 10;
            this.numNumberOfKeys.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // chkCreatepronounceablepasswords
            // 
            this.chkCreatepronounceablepasswords.AutoSize = true;
            this.chkCreatepronounceablepasswords.Checked = true;
            this.chkCreatepronounceablepasswords.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCreatepronounceablepasswords.Location = new System.Drawing.Point(15, 523);
            this.chkCreatepronounceablepasswords.Name = "chkCreatepronounceablepasswords";
            this.chkCreatepronounceablepasswords.Size = new System.Drawing.Size(184, 17);
            this.chkCreatepronounceablepasswords.TabIndex = 11;
            this.chkCreatepronounceablepasswords.Text = "Create pronounceable passwords";
            this.chkCreatepronounceablepasswords.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 555);
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
    }
}

