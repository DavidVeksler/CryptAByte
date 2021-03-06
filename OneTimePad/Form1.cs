﻿using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using CryptAByte.CryptoLibrary.CryptoProviders;

namespace OneTimePad
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //ILMerge.exe /target:OneTimePad /out:OneTimePad2.exe /targetplatform:"v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" OneTimePad.exe CryptAByte.CryptoLibrary.dll

        private void btnGenerateKeys_Click(object sender, EventArgs e)
        {
            GenerateKeys(numKeySize.Value, numNumberOfKeys.Value);
        }

        private void GenerateKeys(decimal keySize, decimal numberOfKeys)
        {
            StringBuilder keys = new StringBuilder();

            for (int i = 1; i < numberOfKeys + 1; i++)
            {
                string key;
                if (chkCreatepronounceablepasswords.Checked)
                {
                    key = PronounceablePasswordGenerator.Generate((int)keySize);
                }
                else
                {
                    key = SymmetricCryptoProvider.GenerateKeyPhrase((int)keySize).TrimEnd(Convert.ToChar("="));
                }

                int groupSize = (int)numGroupSize.Value;

                if (chkGroupIntoPairs.Checked && groupSize > 0)
                {
                    string groupedKey = "";

                    for (int j = 0; j < key.Length; j++)
                    {
                        groupedKey += key[j];

                        if ((j +1) % groupSize == 0)
                        {
                            groupedKey += "-";
                        }
                    }

                    groupedKey = groupedKey.TrimEnd('-');

                    keys.AppendLine(string.Format("{0}, {1}", i, groupedKey));
                }
                else
                {
                    keys.AppendLine(string.Format("{0}, {1}", i, key));
                }

            }

            txtkeys.Text = keys.ToString();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/DavidVeksler/CryptAByte/tree/master/binaries");
        }
    }
}